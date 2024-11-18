using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Utility;
using BepInEx.Logging;

namespace FreeLook
{
    public class FreeLookManager : MonoBehaviour
    {
        private Player _player;
        private Player player
        {
            get
            {
                if (_player == null)
                {
                    _player = GetComponent<Player>();
                }
                return _player;
            }
        }
        private MainCameraControl _mcc;
        private MainCameraControl mcc
        {
            get
            {
                if (_mcc == null)
                {
                    _mcc = MainCameraControl.main;
                }
                return _mcc;
            }
        }
        private Vehicle _vehicle = null;
        public Vehicle vehicle
        {
            get
            {
                if (_vehicle == null)
                {
                    _vehicle = player.GetVehicle();
                }
                return _vehicle;
            }
            set
            {
                _vehicle = value;
            }
        }

        private bool IsFreelyPiloting => player.GetVehicle() != null && !player.GetVehicle().docked && player.GetVehicle().GetPilotingMode();
        private float deadzone => ((float)FreeLookPatcher.config.deadzone) / 100f;

        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        private float xVelocity = 0.0f;
        private float yVelocity = 0.0f;
        const float smoothTime = 0.25f;

        public bool isFreeLooking = false;

        // this controls whether update will be used to "snap back" the cursor to center
        public bool resetCameraFlag = false;

        public bool wasFreelyPilotingLastFrame = false;
        public bool isNewlyInVehicle = false;
        public bool isNewlyOutVehicle = false;

        public bool wasTriggerDownLastFrame = false;
        public bool isTriggerNewlyDown = false;
        public bool isTriggerNewlyUp = false;
        public bool m_IsDocking = false;
        public void Start()
        {
            // check for the existence of The Vehicle Framework
            var type = Type.GetType("VehicleFramework.VehicleManager", false, false);
            if (type != null)
            {
                //Logger.Log("Found VehicleFramework. Compensating.");
            }
        }
        public void Update()
        {
            if (player.playerAnimator.GetBool("cyclops_launch") ||
                player.playerAnimator.GetBool("exosuit_launch") ||
                player.playerAnimator.GetBool("moonpool_launchright") ||
                player.playerAnimator.GetBool("moonpool_launchleft") ||
                player.playerAnimator.GetBool("moonpool_exolaunchleft") ||
                player.playerAnimator.GetBool("moonpool_exolaunchright") ||
                m_IsDocking)
            {
                if (mcc.GetComponentInParent<Player>() != null)
                {
                    mcc.cinematicMode = true;
                }
                return;
            }
            void setInVehicleVars(bool inVehicleThisFrame)
            {
                if (wasFreelyPilotingLastFrame)
                {
                    isNewlyInVehicle = false;
                    if (inVehicleThisFrame)
                    {
                        isNewlyOutVehicle = false;
                        isNewlyInVehicle = false;
                    }
                    else
                    {
                        isNewlyOutVehicle = true;
                        isNewlyInVehicle = false;
                    }
                }
                else
                {
                    isNewlyOutVehicle = false;
                    if (inVehicleThisFrame)
                    {
                        isNewlyOutVehicle = false;
                        isNewlyInVehicle = true;
                    }
                    else
                    {
                        isNewlyOutVehicle = false;
                        isNewlyInVehicle = false;
                    }
                }
                wasFreelyPilotingLastFrame = inVehicleThisFrame;
            }
            void setTriggerStates(bool triggerState)
            {
                if (wasTriggerDownLastFrame)
                {
                    if (triggerState)
                    {
                        isTriggerNewlyUp = false;
                        isTriggerNewlyDown = false;
                    }
                    else
                    {
                        isTriggerNewlyUp = true;
                        isTriggerNewlyDown = false;
                    }
                }
                else
                {
                    if (triggerState)
                    {
                        isTriggerNewlyUp = false;
                        isTriggerNewlyDown = true;
                    }
                    else
                    {
                        isTriggerNewlyUp = false;
                        isTriggerNewlyDown = false;
                    }
                }
                wasTriggerDownLastFrame = triggerState;
            }

            bool isUndocking = player.GetVehicle() != null && player.GetVehicle().docked;
            bool isDocking = (player.GetVehicle() == null) && wasFreelyPilotingLastFrame && player.cinematicModeActive;
            // isDocking also matches on normal vehicle exit...

            bool docked = player.GetVehicle() != null && player.GetVehicle().docked;
            bool piloting = player.IsPiloting();
            bool exited = !IsFreelyPiloting && wasFreelyPilotingLastFrame;

            // if we "just got out," give up the camera right away
            if (exited || isDocking)
            {
                // During Undocking:
                // This case happens HERE
                /*
                    isPilotingUndockedVehicle == FALSE
                    wasInVehicleLastFrame == FALSE
                    player.GetVehicle() == TRUE
                    player.GetVehicle().docked == TRUE
                    player.IsPiloting() == FALSE
                */
                // During Docking:
                // This case happens HERE
                /*
                    isPilotingUndockedVehicle == FALSE
                    wasInVehicleLastFrame == TRUE
                    player.GetVehicle() == NULL
                    player.GetVehicle().docked == ?!? (no vehicle)
                    player.IsPiloting() == FALSE
                */
                // During Regular FreeLooking, after you release the button, and the camera finally snaps back:
                // This case happens LATER IN UPDATE, during ResetCameraRotation
                /*
                    wasInVehicleLastFrame == TRUE
                    player.GetVehicle() == TRUE
                    player.GetVehicle().docked == FALSE
                    player.IsPiloting() == TRUE
                */
                CameraRelinquish(isDocking);
                setInVehicleVars(IsFreelyPiloting);
                return;
            }

            setInVehicleVars(IsFreelyPiloting);

            // For Mimes, print out a hint
            if (isNewlyInVehicle && FreeLookPatcher.config.isHintingEnabled)
            {
                isNewlyInVehicle = false;
                BasicText message = new BasicText(500, 0);
                message.ShowMessage("Hold " + FreeLookPatcher.config.FreeLookKey.ToString() + " to Free Look.", 5);
            }

            if (IsFreelyPiloting)
            {
                vehicle = player.GetVehicle();
                bool triggerState = (Input.GetAxisRaw("ControllerAxis3") > deadzone) || (Input.GetAxisRaw("ControllerAxis3") < -deadzone);
                setTriggerStates(triggerState);
                if ((Input.GetKey(FreeLookPatcher.config.FreeLookKey) || triggerState))
                {
                    ShouldDoEngineAction = true;
                    if (Input.GetKeyDown(FreeLookPatcher.config.FreeLookKey) || isTriggerNewlyDown)
                    {
                        // If we just pressed the FreeLook button, take control of the camera.
                        BeginFreeLook();
                    }
                    // if we're freelooking, control the camera
                    ExecuteFreeLook(vehicle);
                }
                else if (Input.GetKeyUp(FreeLookPatcher.config.FreeLookKey) || isTriggerNewlyUp)
                {
                    ShouldDoEngineAction = false;
                    // If we just released the FreeLook button, flag camera for release
                    BeginReleaseCamera();
                }
                else
                {
                    ShouldDoEngineAction = false;
                }
                if (resetCameraFlag)
                {
                    ShouldDoEngineAction = false;
                    ResetCameraRotation();
                    return;
                }
            }
        }
        internal static bool ShouldDoEngineAction = false;
        public void FixedUpdate()
        {
            if(ShouldDoEngineAction)
            {
                DoEngineAction();
            }
        }
        private void ExecuteFreeLook(Vehicle vehicle)
        {
            resetCameraFlag = false;

            // must add oxygen manually
            OxygenManager oxygenMgr = Player.main.oxygenMgr;
            oxygenMgr.AddOxygen(Time.deltaTime);

            // control the camera
            MoveCamera();
        }
        public void DoEngineAction()
        {
            // add locomotion back in
            if (vehicle.transform.position.y < Ocean.GetOceanLevel() && vehicle.transform.position.y < vehicle.worldForces.waterDepth && !vehicle.precursorOutOfWater)
            {
                Vector3 thisMovementVector;
                if (GameInput.autoMove)
                {
                    thisMovementVector = vehicle.transform.forward;
                }
                else
                {
                    Vector3 myDirection = Vector3.zero;
                    myDirection.z += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
                    myDirection.z -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
                    myDirection.x -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
                    myDirection.x += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
                    myDirection.y += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
                    myDirection.y -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);

                    thisMovementVector = vehicle.transform.forward * myDirection.x +
                                                 vehicle.transform.right * myDirection.z +
                                                 vehicle.transform.up * myDirection.y;
                }
                thisMovementVector.Normalize();
                if (vehicle is SeaMoth)
                {
                    float scalar =
                        Mathf.Abs(thisMovementVector.z) * vehicle.sidewardForce
                        + Mathf.Abs(thisMovementVector.y) * vehicle.verticalForce
                        + Mathf.Max(0f, thisMovementVector.x) * vehicle.forwardForce
                        + Mathf.Max(0f, -thisMovementVector.x) * vehicle.backwardForce;
                    Vector3 force = scalar * thisMovementVector * Time.deltaTime;
                    for (int i = 0; i < vehicle.accelerationModifiers.Length; i++)
                    {
                        vehicle.accelerationModifiers[i].ModifyAcceleration(ref force);
                    }
                    // for some reason, this results in a 1.3x faster seamoth than usual...?
                    // so we clamp it to an observed max :shrug:
                    vehicle.useRigidbody.AddForce(force, ForceMode.VelocityChange);
                    vehicle.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(vehicle.GetComponent<Rigidbody>().velocity, 11.27f);
                }
                else if (vehicle is Exosuit)
                {
                    thisMovementVector = new Vector3(thisMovementVector.x, 0f, thisMovementVector.z);
                    vehicle.GetComponent<Rigidbody>().velocity += thisMovementVector * Time.deltaTime * 10f;
                    vehicle.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(vehicle.GetComponent<Rigidbody>().velocity, 8f);
                }
                else
                {
                    vehicle.GetComponent<Rigidbody>().velocity += thisMovementVector * Time.deltaTime * 10f;
                    vehicle.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(vehicle.GetComponent<Rigidbody>().velocity, 10f);
                }
            }
        }
        private void BeginFreeLook()
        {
            isFreeLooking = true;
            isTriggerNewlyDown = false;
            resetCameraFlag = false;
            // invoke a camera vulnerability
            if (mcc.GetComponentInParent<Player>() != null)
            {
                mcc.cinematicMode = true;
            }

            // We must work with camRotation here for the case where the user is in the exosuit.
            // In this case, camRotation Y can take on values somewhere around [-80,80].
            // We should set rotation to camRotation here,
            // and set the camPivot rotation to zero,
            // so that our subsequent logic has the right origin
            mcc.rotationX = mcc.camRotationX;
            mcc.rotationY = mcc.camRotationY;
            if (mcc.GetComponentInParent<Player>() != null)
            {
                mcc.transform.Find("camOffset/pdaCamPivot").localRotation = Quaternion.identity;
            }

            // exosuit test
            Exosuit exo = vehicle as Exosuit;
            if (exo != null)
            {
                foreach (var tmp in exo.GetComponentsInChildren<RootMotion.FinalIK.AimIK>())
                {
                    tmp.enabled = false;
                }
            }
        }
        private void BeginReleaseCamera()
        {
            resetCameraFlag = true;
            isFreeLooking = false;

            Exosuit exo = vehicle as Exosuit;
            if (exo != null)
            {
                startQuat = exo.transform.Find("exosuit_01/root/geoChildren/lArm_clav").transform.localRotation;
                resetArmStartTime = 0f;
            }
        }

        private Quaternion startQuat = Quaternion.identity;
        private readonly Quaternion endQuat = Quaternion.Euler(2.2f, 0, 0);
        private float resetArmStartTime = 0f;
        internal void ResetCameraRotation()
        {
            mcc.rotationX = Mathf.SmoothDampAngle(mcc.rotationX, 0f, ref xVelocity, smoothTime);
            mcc.rotationY = Mathf.SmoothDampAngle(mcc.rotationY, 0f, ref yVelocity, smoothTime);
            MainCamera.camera.transform.localEulerAngles = new Vector3(-mcc.rotationY, mcc.rotationX, 0f);

            Exosuit exo = vehicle as Exosuit;
            if (exo != null)
            {
                exo.transform.Find("exosuit_01/root/geoChildren/lArm_clav").transform.localRotation = Quaternion.Slerp(startQuat, endQuat, resetArmStartTime);
                exo.transform.Find("exosuit_01/root/geoChildren/rArm_clav").transform.localRotation = Quaternion.Slerp(startQuat, endQuat, resetArmStartTime);
                resetArmStartTime += 4 * Time.deltaTime;
            }

            // if we're not docking or undocking, and
            // if we're close enough to center, snap back and stop executing
            bool isUndocking = player.GetVehicle() != null && player.GetVehicle().docked;
            bool isDocking = (player.GetVehicle() == null) && wasFreelyPilotingLastFrame;
            //if(!isUndocking && !isDocking)
            {
                double threshold = 1;
                if (Mathf.Abs(mcc.rotationX) < threshold && Mathf.Abs(mcc.rotationY) < threshold)
                {
                    mcc.ResetCamera();
                    CameraRelinquish(false);
                    resetCameraFlag = false;
                }
            }
        }
        internal void MoveCamera()
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            if (myLookDelta == Vector2.zero)
            {
                myLookDelta.x -= GameInput.GetAnalogValueForButton(GameInput.Button.LookLeft);
                myLookDelta.x += GameInput.GetAnalogValueForButton(GameInput.Button.LookRight);
                myLookDelta.y += GameInput.GetAnalogValueForButton(GameInput.Button.LookUp);
                myLookDelta.y -= GameInput.GetAnalogValueForButton(GameInput.Button.LookDown);
            }
            mcc.rotationX += myLookDelta.x;
            mcc.rotationY += myLookDelta.y;
            mcc.rotationX = Mathf.Clamp(mcc.rotationX, -100, 100);
            mcc.rotationY = Mathf.Clamp(mcc.rotationY, -80, 80);
            MainCamera.camera.transform.localEulerAngles = new Vector3(-mcc.rotationY, mcc.rotationX, 0f);
        }
        internal void CameraRelinquish(bool isDocking)
        {
            MainCamera.camera.transform.localEulerAngles = Vector3.zero;
            if (isDocking)
            {
                mcc.transform.localEulerAngles = Vector3.zero;
                player.cinematicModeActive = true;
                m_IsDocking = true;
                resetCameraFlag = false;
            }
            IEnumerator RelinquishCameraAfterAnimationEnds()
            {
                //resetCameraFlag = false;
                //mcc.cinematicMode = false;
                //mcc.cinematicMode = isDocking;
                while (player.playerAnimator.GetBool("cyclops_dock") ||
                       player.playerAnimator.GetBool("moonpool_dock") ||
                       player.playerAnimator.GetBool("exosuit_dock") ||
                       player.playerAnimator.GetBool("moonpool_exodock"))
                {
                    yield return null;
                }
                if(m_IsDocking)
                {
                    m_IsDocking = false;
                    player.cinematicModeActive = false;
                }
                resetCameraFlag = false;
                if (mcc.GetComponentInParent<Player>() != null)
                {
                    mcc.cinematicMode = false;
                }

                Exosuit exo = vehicle as Exosuit;
                if (exo != null)
                {
                    foreach (var tmp in exo.GetComponentsInChildren<RootMotion.FinalIK.AimIK>())
                    {
                        tmp.enabled = true;
                        exo.transform.Find("exosuit_01/root/geoChildren/lArm_clav").transform.localRotation = endQuat;
                        exo.transform.Find("exosuit_01/root/geoChildren/rArm_clav").transform.localRotation = endQuat;
                    }
                }
            }
            StartCoroutine(RelinquishCameraAfterAnimationEnds());
        }
    }
}
