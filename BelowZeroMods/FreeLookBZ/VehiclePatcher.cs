using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using LitJson;
using System.Net.NetworkInformation;

namespace FreeLook
{
    public class FreeLookManager
    {
        public bool isFreeLooking = false;

        public FreeLookManager()
        {
            isFreeLooking = false;
        }
    }

    class FreeLookPatcher
    {
        public static void Patch()
        {
            var harmony = new Harmony("com.garyburke.subnautica.freelook.mod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Awake")]
    public class VehicleAwakePatch
    {
        public static FreeLookManager myFreeMan;
        public static Options Options = new Options();

        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            // initialize the roll manager
            myFreeMan = new FreeLookManager();
            OptionsPanelHandler.RegisterModOptions(Options);
            return true;
        }
    }


    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Update")]
    public class VehicleUpdatePatch
    { 
        public static void moveCamera(Vehicle mySeamoth)
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
        public static bool Prefix(Vehicle __instance)
        {
            var mainCam = MainCameraControl.main;
            //var cameraToPlayerMan = CameraToPlayerManager.main;

            bool inVehicleNow = (Player.main.inSeamoth || Player.main.inExosuit);

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

            if (isNewlyInVehicle)
            {
                isNewlyInVehicle = false;
                //BasicText message = new BasicText();
                //message.ShowMessage("This Message Will Fade In 10 Seconds", 10);
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

            // add locomotion back in
            if ((Input.GetKey(Options.freeLookKey) || isTriggerDown) && __instance == Player.main.currentMountedVehicle)// and we're using controller)
            {
                Vector3 myDirection = Vector3.zero;
                myDirection.z = Input.GetAxis("ControllerAxis1");
                myDirection.x = -Input.GetAxis("ControllerAxis2");
                myDirection.y =
                    GameInput.GetButtonHeld(GameInput.Button.MoveUp) ?
                    (GameInput.GetButtonHeld(GameInput.Button.MoveDown) ? 0 : 1) :
                    (GameInput.GetButtonHeld(GameInput.Button.MoveDown) ? -1 : 0);

                Vector3 myModDir = __instance.transform.forward * myDirection.x +
                                    __instance.transform.right * myDirection.z +
                                    __instance.transform.up * myDirection.y;

                myModDir = Vector3.Normalize(myModDir);

                __instance.GetComponent<Rigidbody>().velocity += myModDir * Time.deltaTime * 10f;
                __instance.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(__instance.GetComponent<Rigidbody>().velocity, 10f);
            }


            void cameraRelinquish()
            {
                mainCam.ResetCamera();
                mainCam.cinematicMode = false;
                mainCam.lookAroundMode = true;
                VehicleAwakePatch.myFreeMan.isFreeLooking = false;
            }

            if (Input.GetKeyDown(Options.freeLookKey) || isTriggerNewlyDown)
            {
                Debug.Log("FreeLook: button pressed. Taking control of the camera.");
                VehicleAwakePatch.myFreeMan.isFreeLooking = true;
                isTriggerNewlyDown = false;

                resetCameraFlag = false;
                // invoke a camera vulnerability
                mainCam.cinematicMode = true;
                mainCam.lookAroundMode = false;
                return false;
            }
            else if (Input.GetKeyUp(Options.freeLookKey) || isTriggerNewlyUp)
            {
                isTriggerNewlyUp = false;
                Debug.Log("FreeLook: button released. Relinquishing control of the camera.");
                resetCameraFlag = true;
            }
            if (!resetCameraFlag && (Input.GetKey(Options.freeLookKey) || isTriggerDown))
            {
                resetCameraFlag = false;
                moveCamera(Player.main.currentMountedVehicle);
                // adding oxygen is something vehicle.update would usually do,
                // so we do it naively here as well.
                // I'm not sure that Time.deltaTime seconds worth of oxygen per frame is the right amount...
                OxygenManager oxygenMgr = Player.main.oxygenMgr;
                oxygenMgr.AddOxygen(Time.deltaTime);
                return false;
            }

            if (resetCameraFlag)
            {
                mainCam.rotationX = Mathf.SmoothDampAngle(mainCam.rotationX, 0f, ref xVelocity, smoothTime);
                mainCam.rotationY = Mathf.SmoothDampAngle(mainCam.rotationY, 0f, ref yVelocity, smoothTime);

                mainCam.camRotationX = mainCam.rotationX;
                mainCam.camRotationY = mainCam.rotationY;

                mainCam.cameraOffsetTransform.localEulerAngles = new Vector3(-mainCam.camRotationY, mainCam.camRotationX, 0);

                double threshold = 1;
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
