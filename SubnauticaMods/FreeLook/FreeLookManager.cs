using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SMLHelper.V2.Utility;

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
                    _mcc = player.GetComponentInChildren<MainCameraControl>();
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


        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        private float xVelocity = 0.0f;
        private float yVelocity = 0.0f;
        const float smoothTime = 0.25f;

        public bool isFreeLooking = false;

        // this controls whether update will be used to "snap back" the cursor to center
        public bool resetCameraFlag = false;

        public bool wasInVehicleLastFrame = false;
        public bool isNewlyInVehicle = false;
        public bool isNewlyOutVehicle = false;

        public bool wasTriggerDownLastFrame = false;
        public bool isTriggerNewlyDown = false;
        public bool isTriggerNewlyUp = false;

        public void Update()
        {
            if (player.playerAnimator.GetBool("cyclops_launch") ||
                player.playerAnimator.GetBool("exosuit_launch") ||
                player.playerAnimator.GetBool("moonpool_launchright") ||
                player.playerAnimator.GetBool("moonpool_launchleft") ||
                player.playerAnimator.GetBool("moonpool_exolaunchleft") ||
                player.playerAnimator.GetBool("moonpool_exolaunchright"))
            {
                mcc.cinematicMode = true;
                return;
            }
            void setInVehicleVars(bool inVehicleThisFrame)
            {
                if (wasInVehicleLastFrame)
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
                wasInVehicleLastFrame = inVehicleThisFrame;
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
            bool isPilotingUndockedVehicle = player.GetVehicle() != null && !player.GetVehicle().docked && player.IsPiloting();

            if ((!isPilotingUndockedVehicle && wasInVehicleLastFrame) || (player.GetVehicle() != null && player.GetVehicle().docked))
            {
                // if we just got out, give up the camera right away
                CameraRelinquish();
                setInVehicleVars(isPilotingUndockedVehicle);
                return;
            }

            setInVehicleVars(isPilotingUndockedVehicle);

            // For Mimes, print out a hint
            if (isNewlyInVehicle && FreeLookPatcher.Config.isHintingEnabled)
            {
                isNewlyInVehicle = false;
                BasicText message = new BasicText(500, 0);
                message.ShowMessage("Hold " + FreeLookPatcher.Config.FreeLookKey.ToString() + " to Free Look.", 5);
            }

            if (isPilotingUndockedVehicle)
            {
                vehicle = player.GetVehicle();
                // TODO: abstract these constants away
                bool triggerState = (Input.GetAxisRaw("ControllerAxis3") > 0) || (Input.GetAxisRaw("ControllerAxis3") < 0);
                setTriggerStates(triggerState);
                if ((Input.GetKey(FreeLookPatcher.Config.FreeLookKey) || triggerState))
                {
                    if (Input.GetKeyDown(FreeLookPatcher.Config.FreeLookKey) || isTriggerNewlyDown)
                    {
                        // If we just pressed the FreeLook button, take control of the camera.
                        BeginFreeLook();
                    }
                    // if we're freelooking, control the camera
                    ExecuteFreeLook(vehicle);
                }
                else if (Input.GetKeyUp(FreeLookPatcher.Config.FreeLookKey) || isTriggerNewlyUp)
                {
                    // If we just released the FreeLook button, flag camera for release
                    BeginReleaseCamera();
                }
                if (resetCameraFlag)
                {
                    ResetCameraRotation();
                    return;
                }
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

            // add locomotion back in
            Vector3 myDirection = Vector3.zero;
            myDirection.z = Input.GetAxis("ControllerAxis1");
            myDirection.x = -Input.GetAxis("ControllerAxis2");
            myDirection.y =
                GameInput.GetButtonHeld(GameInput.Button.MoveUp) ?
                (GameInput.GetButtonHeld(GameInput.Button.MoveDown) ? 0 : 1) :
                (GameInput.GetButtonHeld(GameInput.Button.MoveDown) ? -1 : 0);

            Vector3 myModDir = vehicle.transform.forward * myDirection.x +
                                vehicle.transform.right * myDirection.z +
                                vehicle.transform.up * myDirection.y;

            myModDir = Vector3.Normalize(myModDir);

            vehicle.GetComponent<Rigidbody>().velocity += myModDir * Time.deltaTime * 10f;
            vehicle.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(vehicle.GetComponent<Rigidbody>().velocity, 10f);
        }
        private void BeginFreeLook()
        {
            isFreeLooking = true;
            isTriggerNewlyDown = false;
            resetCameraFlag = false;
            // invoke a camera vulnerability
            mcc.cinematicMode = true;

            // compensate for VehicleFramework
            VehicleFramework.ModVehicle mv = vehicle as VehicleFramework.ModVehicle;
            if (mv != null)
            {
                mv.engine.canControlRotation = false;
            }
        }
        private void BeginReleaseCamera()
        {
            resetCameraFlag = true;
            ResetCameraRotation();
            isFreeLooking = false;
            // compensate for VehicleFramework
            VehicleFramework.ModVehicle mv = vehicle as VehicleFramework.ModVehicle;
            if (mv != null)
            {
                mv.engine.canControlRotation = true;
            }
        }
        internal void ResetCameraRotation()
        {
            mcc.rotationX = Mathf.SmoothDampAngle(mcc.rotationX, 0f, ref xVelocity, smoothTime);
            mcc.rotationY = Mathf.SmoothDampAngle(mcc.rotationY, 0f, ref yVelocity, smoothTime);
            mcc.transform.localEulerAngles = new Vector3(-mcc.rotationY, mcc.rotationX, 0);
            // if we're close enough to center, snap back and stop executing
            double threshold = 1;
            if (Mathf.Abs(mcc.rotationX) < threshold && Mathf.Abs(mcc.rotationY) < threshold)
            {
                mcc.ResetCamera();
                CameraRelinquish();
                resetCameraFlag = false;
            }
        }
        internal void MoveCamera()
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            if (myLookDelta == Vector2.zero)
            {
                // TODO: abstract this shit away
                myLookDelta.x = Input.GetAxis("ControllerAxis4");
                myLookDelta.y = -Input.GetAxis("ControllerAxis5");
            }
            mcc.rotationX += myLookDelta.x;
            mcc.rotationY += myLookDelta.y;
            mcc.rotationX = Mathf.Clamp(mcc.rotationX, -100, 100);
            mcc.rotationY = Mathf.Clamp(mcc.rotationY, -80, 80);
            mcc.transform.localEulerAngles = new Vector3(-mcc.rotationY, mcc.rotationX, 0f);
        }
        internal void CameraRelinquish()
        {
            IEnumerator RelinquishCameraAfterAnimationEnds()
            {
                while (player.playerAnimator.GetBool("cyclops_dock") ||
                       player.playerAnimator.GetBool("moonpool_dock") ||
                       player.playerAnimator.GetBool("exosuit_dock") ||
                       player.playerAnimator.GetBool("moonpool_exodock"))
                {
                    yield return null;
                }
                resetCameraFlag = false;
                mcc.cinematicMode = false;
            }
            StartCoroutine(RelinquishCameraAfterAnimationEnds());
        }
    }
}
