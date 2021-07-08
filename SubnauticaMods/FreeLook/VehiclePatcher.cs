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
    /*
    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Awake")]
    public class VehicleAwakePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            FreeLookManager.reset();
            return true;
        }
    }
    */

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Update")]
    public class VehicleUpdatePatch
    {
        public static void setInVehicleVars(bool inVehicleThisFrame)
        {
            if (FreeLookManager.wasInVehicleLastFrame)
            {
                if (inVehicleThisFrame)
                {
                    // do nothing
                }
                else
                {
                    FreeLookManager.wasInVehicleLastFrame = false;
                }
            }
            else
            {
                if (inVehicleThisFrame)
                {
                    FreeLookManager.wasInVehicleLastFrame = true;
                    FreeLookManager.isNewlyInVehicle = true;
                }
                else
                {
                    // do nothing
                }
            }
        }
        public static void setTriggerStates(bool triggerState)
        {

            if (FreeLookManager.wasTriggerDownLastFrame)
            {
                if (triggerState)
                {
                    //do nothing
                }
                else
                {
                    FreeLookManager.isTriggerNewlyUp = true;
                    FreeLookManager.wasTriggerDownLastFrame = false;
                }
            }
            else
            {
                if (triggerState)
                {
                    FreeLookManager.wasTriggerDownLastFrame = true;
                    FreeLookManager.isTriggerNewlyDown = true;
                }
                else
                {
                    //do nothing
                }
            }
        }

        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            if (Player.main.currentMountedVehicle == null || __instance.docked)
            {
                return true;
            }

            bool inVehicleThisFrame = Player.main.inSeamoth || Player.main.inExosuit;
            setInVehicleVars(inVehicleThisFrame);

            // if we just got out, give up the camera right away
            if (!inVehicleThisFrame && FreeLookManager.wasInVehicleLastFrame)
            {
                FreeLookManager.cameraRelinquish();
                return true;
            }
            
            // if we're not in a vehicle, and we weren't last frame either, return
            if(!inVehicleThisFrame)
            {
                return true;
            }

            // if we're resetting the camera rotation, do it right away and then return
            if (FreeLookManager.resetCameraFlag)
            {
                FreeLookManager.resetCameraRotation();
                // need to retain control in order to finish snapping back to center
                return false;
            }


            bool triggerState = (Input.GetAxisRaw("ControllerAxis3") > 0) || (Input.GetAxisRaw("ControllerAxis3") < 0);
            setTriggerStates(triggerState);

            // For Mimes, print out a hint
            if (FreeLookManager.isNewlyInVehicle && FreeLookPatcher.Config.isHintingEnabled)
            {
                FreeLookManager.isNewlyInVehicle = false;
                BasicText message = new BasicText(500,0);
                message.ShowMessage("Hold " + FreeLookPatcher.Config.FreeLookKey.ToString() + " to Free Look.", 5);
            }



            //=====================
            // begin control flow
            //=====================


            // If we just pressed the FreeLook button, take control of the camera.
            if (Input.GetKeyDown(FreeLookPatcher.Config.FreeLookKey) || FreeLookManager.isTriggerNewlyDown)
            {
                //Debug.Log("FreeLook: button pressed. Taking control of the camera.");

                FreeLookManager.isFreeLooking = true;
                FreeLookManager.isTriggerNewlyDown = false;
                FreeLookManager.resetCameraFlag = false;

                // invoke a camera vulnerability
                MainCameraControl mainCam = MainCameraControl.main;
                mainCam.cinematicMode = true;
                //mainCam.lookAroundMode = false;
            }

            // If we just released the FreeLook button, relinquish control of the camera.
            if (Input.GetKeyUp(FreeLookPatcher.Config.FreeLookKey) || FreeLookManager.isTriggerNewlyUp)
            {
                //Debug.Log("FreeLook: button released. Relinquishing control of the camera.");
                FreeLookManager.isTriggerNewlyUp = false;
                FreeLookManager.resetCameraFlag = true;
                return false;
            }

            // if we're freelooking, control the camera
            if ((Input.GetKey(FreeLookPatcher.Config.FreeLookKey) || triggerState) && __instance == Player.main.currentMountedVehicle)
            {
                FreeLookManager.resetCameraFlag = false;

                // must add oxygen manually
                OxygenManager oxygenMgr = Player.main.oxygenMgr;
                oxygenMgr.AddOxygen(Time.deltaTime);

                // control the camera
                FreeLookManager.moveCamera(Player.main.currentMountedVehicle);

                // add locomotion back in
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
                
                return false;
            }

            // nothing from the key
            // we're not freelooking
            // the camera has been reset
            return true;
        }
    }
}
