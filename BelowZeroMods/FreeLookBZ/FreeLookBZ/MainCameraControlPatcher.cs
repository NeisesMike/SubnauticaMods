using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace FreeLook
{
    [HarmonyPatch(typeof(MainCameraControl))]
    [HarmonyPatch("Update")]
    [HarmonyPatch(nameof(MainCameraControl.OnUpdate))]
    public class MainCameraControlUpdatePatcher
    {
        public static void moveCamera()
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            if (myLookDelta == Vector2.zero)
            {
                myLookDelta.x = Input.GetAxis("ControllerAxis4");
                myLookDelta.y = -Input.GetAxis("ControllerAxis5");
            }

            MainCameraControl mainCam = MainCameraControl.main;
            mainCam.rotationX += myLookDelta.x;
            mainCam.rotationY += myLookDelta.y;
            mainCam.rotationX = Mathf.Clamp(mainCam.rotationX, -100, 100);
            mainCam.rotationY = Mathf.Clamp(mainCam.rotationY, mainCam.minimumY, mainCam.maximumY);

            //mainCam.cameraUPTransform.localEulerAngles = new Vector3(Mathf.Min(0f, -mainCam.rotationY), 0f, 0f);

            mainCam.transform.localEulerAngles = new Vector3(-mainCam.rotationY, mainCam.rotationX, 0f);
        }

        // this controls whether update will be used to "snap back" the cursor to center
        static bool resetCameraFlag = false;
        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        static float xVelocity = 0.0f;
        static float yVelocity = 0.0f;
        // this is how long it takes the cursor to snap back to center
        static float smoothTime = 0.25f;

        static bool releaseFlag = false;
        static Vector3 cameraOffsetTransformPosition = new Vector3(0, 0, 0);

        static bool isTriggerDown = false;
        static bool isTriggerNewlyDown = false;
        static bool isTriggerNewlyUp = false;

        static bool isInVehicle = false;
        static bool isNewlyInVehicle = false;

        [HarmonyPrefix]
        public static bool Prefix(MainCameraControl __instance)
        {
            var mainCam = MainCameraControl.main;
            //var cameraToPlayerMan = CameraToPlayerManager.main;

            bool inVehicleNow = (Player.main.IsPilotingSeatruck() || Player.main.inExosuit);

            if (isInVehicle)
            {
                if (inVehicleNow)
                {
                    // do nothing
                }
                else
                {
                    isInVehicle = false;
                }
            }
            else
            {
                if (inVehicleNow)
                {
                    isInVehicle = true;
                    isNewlyInVehicle = true;
                }
                else
                {
                    // do nothing
                }
            }

            if (!inVehicleNow)
            {
                if (releaseFlag)
                {
                    cameraRelinquish();
                    releaseFlag = false;
                }
                return true;
            }
            releaseFlag = true;

            if (isNewlyInVehicle && FreeLookPatcher.FLConfig.isHintingEnabled)
            {
                isNewlyInVehicle = false;
                Logger.Output("Press " + FreeLookPatcher.FLConfig.FreeLookKey.ToString() + " to FreeLook.");
            }

            if (Player.main.motorMode == Player.MotorMode.Seaglide)
            {
                // hack the controls
            }

            bool triggerState = (Input.GetAxisRaw("ControllerAxis3") > 0) || (Input.GetAxisRaw("ControllerAxis3") < 0);
            if (isTriggerDown)
            {
                if (triggerState)
                {
                    //do nothing
                }
                else
                {
                    isTriggerDown = false;
                    isTriggerNewlyUp = true;
                }
            }
            else
            {
                if (triggerState)
                {
                    isTriggerDown = true;
                    isTriggerNewlyDown = true;
                }
                else
                {
                    //do nothing
                }
            }

            void cameraRelinquish()
            {
                mainCam.ResetCamera();
                mainCam.cinematicMode = false;
                mainCam.lookAroundMode = true;
                FreeLookPatcher.isFreeLooking = false;
            }

            if (Input.GetKeyDown(FreeLookPatcher.FLConfig.FreeLookKey) || isTriggerNewlyDown)
            {
                FreeLookPatcher.isFreeLooking = true;
                isTriggerNewlyDown = false;

                resetCameraFlag = false;
                // invoke a camera vulnerability
                mainCam.cinematicMode = true;
                mainCam.lookAroundMode = false;
                return false;
            }
            else if (Input.GetKeyUp(FreeLookPatcher.FLConfig.FreeLookKey) || isTriggerNewlyUp)
            {
                isTriggerNewlyUp = false;
                resetCameraFlag = true;
            }
            if (!resetCameraFlag && (Input.GetKey(FreeLookPatcher.FLConfig.FreeLookKey) || isTriggerDown))
            {
                resetCameraFlag = false;
                moveCamera();
                // adding oxygen is something vehicle.update would usually do,
                // so we do it naively here as well.
                // I'm not sure that Time.deltaTime seconds worth of oxygen per frame is the right amount...
                OxygenManager oxygenMgr = Player.main.oxygenMgr;
                oxygenMgr.AddOxygen(Time.deltaTime);
                return false;
            }

            if (resetCameraFlag)
            {
                double threshold = 1;

                if (threshold <= Mathf.Abs(mainCam.rotationX))
                {
                    mainCam.rotationX = Mathf.SmoothDampAngle(mainCam.rotationX, 0f, ref xVelocity, smoothTime);
                    mainCam.camRotationX = mainCam.rotationX;
                }
                if (threshold <= Mathf.Abs(mainCam.rotationY))
                {
                    mainCam.rotationY = Mathf.SmoothDampAngle(mainCam.rotationY, 0f, ref yVelocity, smoothTime);
                    mainCam.camRotationY = mainCam.rotationY;
                }

                mainCam.transform.localEulerAngles = new Vector3(-mainCam.camRotationY, mainCam.camRotationX, 0);

                if (Mathf.Abs(mainCam.camRotationX) < threshold && Mathf.Abs(mainCam.camRotationY) < threshold)
                {
                    cameraRelinquish();
                    resetCameraFlag = false;
                }
                // need to retain control in order to finish snapping back to center
                return false;
            }
            // nothing from the key and the camera has been reset, so we don't need control
            //cameraRelinquish();
            return true;
        }
    }
}
