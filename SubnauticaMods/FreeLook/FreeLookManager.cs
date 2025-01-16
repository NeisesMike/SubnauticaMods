using System.Collections;
using UnityEngine;
using Nautilus.Utility;

namespace FreeLook
{
    public class FreeLookManager : MonoBehaviour
    {
        internal bool isToggled = false;
        internal bool isFreeLooking = false;
        private Player MyPlayer => GetComponent<Player>();
        private MainCameraControl MCC => MainCameraControl.main;
        private Vehicle Vehicle => MyPlayer?.GetVehicle();
        private bool IsFreelyPiloting => Vehicle != null && !Vehicle.docked && Vehicle.GetPilotingMode();
        private float Deadzone => ((float)FreeLookPatcher.config.deadzone) / 100f;
        private bool IsTriggerHeld => (Input.GetAxisRaw("ControllerAxis3") > Deadzone) || (Input.GetAxisRaw("ControllerAxis3") < -Deadzone);
        private bool IsControlHeld => Input.GetKey(FreeLookPatcher.config.FreeLookKey) || IsTriggerHeld;
        private bool IsControlDown => Input.GetKeyDown(FreeLookPatcher.config.FreeLookKey) || isTriggerNewlyDown;
        private bool IsControlUp => Input.GetKeyUp(FreeLookPatcher.config.FreeLookKey) || isTriggerNewlyUp;

        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        private float xVelocity = 0.0f;
        private float yVelocity = 0.0f;
        private const float smoothTime = 0.25f;

        private Quaternion startQuat = Quaternion.identity;
        private readonly Quaternion endQuat = Quaternion.Euler(2.2f, 0, 0);
        private float resetArmStartTime = 0f;

        // this controls whether update will be used to "snap back" the cursor to center
        private bool resetCameraFlag = false;

        private bool wasFreelyPilotingLastFrame = false;
        private bool isNewlyInVehicle = false;

        private bool wasTriggerDownLastFrame = false;
        private bool isTriggerNewlyDown = false;
        private bool isTriggerNewlyUp = false;
        private bool m_IsDocking = false;

        private void SetInVehicleVars(bool inVehicleThisFrame)
        {
            if (wasFreelyPilotingLastFrame)
            {
                isNewlyInVehicle = false;
            }
            else
            {
                isNewlyInVehicle = inVehicleThisFrame;
            }
            wasFreelyPilotingLastFrame = inVehicleThisFrame;
        }
        private void HandleTriggers(bool triggerState)
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
        public void Update()
        {
            if (MyPlayer.playerAnimator.GetBool("cyclops_launch") ||
                MyPlayer.playerAnimator.GetBool("exosuit_launch") ||
                MyPlayer.playerAnimator.GetBool("moonpool_launchright") ||
                MyPlayer.playerAnimator.GetBool("moonpool_launchleft") ||
                MyPlayer.playerAnimator.GetBool("moonpool_exolaunchleft") ||
                MyPlayer.playerAnimator.GetBool("moonpool_exolaunchright") ||
                m_IsDocking)
            {
                if (MCC.GetComponentInParent<Player>() != null)
                {
                    MCC.cinematicMode = true;
                }
                return;
            }

            bool isUndocking = MyPlayer.GetVehicle() != null && MyPlayer.GetVehicle().docked;
            bool isDocking = (MyPlayer.GetVehicle() == null) && wasFreelyPilotingLastFrame && MyPlayer.cinematicModeActive;
            // isDocking also matches on normal vehicle exit...

            bool docked = MyPlayer.GetVehicle() != null && MyPlayer.GetVehicle().docked;
            bool piloting = MyPlayer.IsPiloting();
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
                SetInVehicleVars(IsFreelyPiloting);
                return;
            }

            SetInVehicleVars(IsFreelyPiloting);

            // For Mimes, print out a hint
            if (isNewlyInVehicle && FreeLookPatcher.config.isHintingEnabled)
            {
                isNewlyInVehicle = false;
                BasicText message = new BasicText(500, 0);
                message.ShowMessage("Hold " + FreeLookPatcher.config.FreeLookKey.ToString() + " to Free Look.", 5);
            }

            TryFreeLooking();
        }
        private void TryNormalFreeLooking()
        {
            if (IsControlDown)
            {
                // take control of the camera.
                BeginFreeLook();
            }
            if (IsControlHeld)
            {
                // control the camera
                ExecuteFreeLook();
                return;
            }
            if (IsControlUp)
            {
                // flag camera for release
                BeginReleaseCamera();
            }
        }
        private void TryToggleFreeLooking()
        {
            if (IsControlDown)
            {
                isToggled = !isToggled;
                if(isToggled)
                {
                    BeginFreeLook();
                }
                else
                {
                    BeginReleaseCamera();
                }
            }
            if (isToggled)
            {
                ExecuteFreeLook();
            }
        }
        private void TryFreeLooking()
        {
            if (IsFreelyPiloting)
            {
                HandleTriggers(IsTriggerHeld);
                if(FreeLookPatcher.config.isToggle)
                {
                    TryToggleFreeLooking();
                }
                else
                {
                    TryNormalFreeLooking();
                }
            }
            if (resetCameraFlag)
            {
                ResetCameraRotation();
            }
        }
        private void ExecuteFreeLook()
        {
            if (!UWE.FreezeTime.HasFreezers())
            {
                resetCameraFlag = false;
                MyPlayer.oxygenMgr.AddOxygen(Time.deltaTime); // FreeLook overrides Vehicle.Update, so we have to add oxygen here.
                MoveCamera();
            }
        }
        private void BeginFreeLook()
        {
            isFreeLooking = true;
            isTriggerNewlyDown = false;
            resetCameraFlag = false;
            // invoke a camera vulnerability
            if (MCC.GetComponentInParent<Player>() != null)
            {
                MCC.cinematicMode = true;
            }

            // We must work with camRotation here for the case where the user is in the exosuit.
            // In this case, camRotation Y can take on values somewhere around [-80,80].
            // We should set rotation to camRotation here,
            // and set the camPivot rotation to zero,
            // so that our subsequent logic has the right origin
            MCC.rotationX = MCC.camRotationX;
            MCC.rotationY = MCC.camRotationY;
            if (MCC.GetComponentInParent<Player>() != null)
            {
                MCC.transform.Find("camOffset/pdaCamPivot").localRotation = Quaternion.identity;
            }

            // exosuit test
            Exosuit exo = Vehicle as Exosuit;
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

            Exosuit exo = Vehicle as Exosuit;
            if (exo != null)
            {
                startQuat = exo.transform.Find("exosuit_01/root/geoChildren/lArm_clav").transform.localRotation;
                resetArmStartTime = 0f;
            }
        }
        private void ResetCameraRotation()
        {
            MCC.rotationX = Mathf.SmoothDampAngle(MCC.rotationX, 0f, ref xVelocity, smoothTime);
            MCC.rotationY = Mathf.SmoothDampAngle(MCC.rotationY, 0f, ref yVelocity, smoothTime);
            MainCamera.camera.transform.localEulerAngles = new Vector3(-MCC.rotationY, MCC.rotationX, 0f);

            Exosuit exo = Vehicle as Exosuit;
            if (exo != null)
            {
                exo.transform.Find("exosuit_01/root/geoChildren/lArm_clav").transform.localRotation = Quaternion.Slerp(startQuat, endQuat, resetArmStartTime);
                exo.transform.Find("exosuit_01/root/geoChildren/rArm_clav").transform.localRotation = Quaternion.Slerp(startQuat, endQuat, resetArmStartTime);
                resetArmStartTime += 4 * Time.deltaTime;
            }

            // if we're not docking or undocking, and
            // if we're close enough to center, snap back and stop executing
            bool isUndocking = MyPlayer.GetVehicle() != null && MyPlayer.GetVehicle().docked;
            bool isDocking = (MyPlayer.GetVehicle() == null) && wasFreelyPilotingLastFrame;
            //if(!isUndocking && !isDocking)
            {
                double threshold = 0.01f;
                if (Mathf.Abs(MCC.rotationX) < threshold && Mathf.Abs(MCC.rotationY) < threshold)
                {
                    HardReset();
                }
            }
        }
        private void MoveCamera()
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            if (myLookDelta == Vector2.zero)
            {
                myLookDelta.x -= GameInput.GetAnalogValueForButton(GameInput.Button.LookLeft);
                myLookDelta.x += GameInput.GetAnalogValueForButton(GameInput.Button.LookRight);
                myLookDelta.y += GameInput.GetAnalogValueForButton(GameInput.Button.LookUp);
                myLookDelta.y -= GameInput.GetAnalogValueForButton(GameInput.Button.LookDown);
            }
            MCC.rotationX += myLookDelta.x;
            MCC.rotationY += myLookDelta.y;
            MCC.rotationX = Mathf.Clamp(MCC.rotationX, -100, 100);
            MCC.rotationY = Mathf.Clamp(MCC.rotationY, -80, 80);
            MainCamera.camera.transform.localEulerAngles = new Vector3(-MCC.rotationY, MCC.rotationX, 0f);
        }
        private void CameraRelinquish(bool isDocking)
        {
            MainCamera.camera.transform.localEulerAngles = Vector3.zero;
            if (isDocking)
            {
                MCC.transform.localEulerAngles = Vector3.zero;
                MyPlayer.cinematicModeActive = true;
                m_IsDocking = true;
                resetCameraFlag = false;
            }
            IEnumerator RelinquishCameraAfterAnimationEnds()
            {
                //resetCameraFlag = false;
                //mcc.cinematicMode = false;
                //mcc.cinematicMode = isDocking;
                while (MyPlayer.playerAnimator.GetBool("cyclops_dock") ||
                       MyPlayer.playerAnimator.GetBool("moonpool_dock") ||
                       MyPlayer.playerAnimator.GetBool("exosuit_dock") ||
                       MyPlayer.playerAnimator.GetBool("moonpool_exodock"))
                {
                    yield return null;
                }
                if(m_IsDocking)
                {
                    m_IsDocking = false;
                    MyPlayer.cinematicModeActive = false;
                }
                resetCameraFlag = false;
                if (MCC.GetComponentInParent<Player>() != null)
                {
                    MCC.cinematicMode = false;
                }

                Exosuit exo = Vehicle as Exosuit;
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
            UWE.CoroutineHost.StartCoroutine(RelinquishCameraAfterAnimationEnds());
        }
        internal void HardReset()
        {
            MCC.ResetCamera();
            CameraRelinquish(false);
            resetCameraFlag = false;
            isFreeLooking = false;
            isToggled = false;
        }
        private void OnApplicationFocus(bool focus)
        {
            if(IsFreelyPiloting)
            {
                HardReset();
            }
        }
    }
}
