using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Harmony;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Net.NetworkInformation;

namespace FreeLook
{
    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Update")]

    public class FreeLookPatcher
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.garyburke.subnautica.freelook.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Initialise();
        }

        public static Options Options = new Options();
        public static void Initialise()
        {
            OptionsPanelHandler.RegisterModOptions(Options);
        }

        public static void moveCamera(Vehicle mySeamoth)
        {
            Vector2 vector = GameInput.GetLookDelta();
            MainCameraControl mainCam = MainCameraControl.main;
            mainCam.rotationX += vector.x;
            mainCam.rotationY += vector.y;
            mainCam.rotationY = Mathf.Clamp(mainCam.rotationY, mainCam.minimumY, mainCam.maximumY);
            mainCam.transform.localEulerAngles = new Vector3(-mainCam.rotationY, mainCam.rotationX, 0f);
        }

        // this controls whether update will be used to "snap back" the cursor to center
        static bool resetCameraFlag = false;
        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        static float xVelocity      = 0.0f;
        static float yVelocity      = 0.0f;
        // this is how long it takes the cursor to snap back to center
        static float smoothTime     = 0.5f;

        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            if( !(Player.main.inSeamoth || Player.main.inExosuit) )
            {
                return true;
            }

            var mainCam = MainCameraControl.main;

            void cameraRelinquish()
            {
                mainCam.ResetCamera();
                mainCam.cinematicMode = false;
                mainCam.lookAroundMode = true;
            }

            if (Input.GetKeyDown(Options.freeLookKey))
            {
                resetCameraFlag = false;
                // invoke a camera vulnerability
                mainCam.cinematicMode = true;
                mainCam.lookAroundMode = false;
                return false;
            }
            else if (Input.GetKeyUp(Options.freeLookKey))
            {
                resetCameraFlag = true;
            }
            if ( !resetCameraFlag && Input.GetKey(Options.freeLookKey))
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

                double threshold = 0.001;
                if( Mathf.Abs(mainCam.camRotationX) < threshold && Mathf.Abs(mainCam.camRotationY) < threshold )
                {
                    cameraRelinquish();
                    resetCameraFlag = false;
                }
                // need to retain control in order to finish snapping back to center
                return false;
            }
            // nothing from the key and the camera has been reset, so we don't need control
            cameraRelinquish();
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
        }
    }
}
