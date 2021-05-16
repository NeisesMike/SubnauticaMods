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
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace FreeLook
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeLookZero] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[FreeLookZero] " + string.Format(format, args));
        }
    }

    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;
    }

    class FreeLookPatcher
    {
        internal static MyConfig Config { get; private set; }
        public static bool isFreeLooking;
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            isFreeLooking = false;

            var harmony = new Harmony("com.garyburke.subnautica.freelookzero.mod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("Update")]
    public class SeaTruckMotorUpdatePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, float ___afterBurnerTime, bool ___waitForDocking, GameObject ___inputStackDummy, float ___animAccel, FMOD.Studio.PARAMETER_ID ___velocityParamIndex,
                FMOD.Studio.PARAMETER_ID ___depthParamIndex, FMOD.Studio.PARAMETER_ID ___rpmParamIndex, FMOD.Studio.PARAMETER_ID ___turnParamIndex, FMOD.Studio.PARAMETER_ID ___upgradeParamIndex,
                FMOD.Studio.PARAMETER_ID ___damagedParamIndex, Animator ___animator)
        {
            if (!FreeLookPatcher.isFreeLooking)
            {
                Logger.Log("Motor as usual");
                return true;
            }
            else
            {
                Logger.Log("Motor differently");
                if (__instance.afterBurnerActive && Time.time > ___afterBurnerTime)
                {
                    __instance.afterBurnerActive = false;
                }
                __instance.UpdateDrag();
                if (__instance.piloting && __instance.useRigidbody != null && !__instance.IsBusyAnimating() && !___waitForDocking)
                {
                    if ((__instance.truckSegment.isMainCab ? (AvatarInputHandler.main.IsEnabled() && !Player.main.GetPDA().isInUse) : ___inputStackDummy.activeInHierarchy) && GameInput.GetButtonDown(GameInput.Button.Exit))
                    {
                        __instance.StopPiloting(false, false, false);
                    }
                    else if (!__instance.truckSegment.isMainCab && GameInput.GetButtonDown(GameInput.Button.PDA))
                    {
                        __instance.StopPiloting(false, false, false);
                        __instance.OpenPDADelayed(0.7f);
                    }
                    else if (!__instance.truckSegment.isMainCab && Player.main.transform.position.y > -1.5f)
                    {
                        __instance.StopPiloting(false, false, false);
                        float d = 5f;
                        __instance.useRigidbody.AddForce(-Vector3.up * d, ForceMode.VelocityChange);
                    }
                    else if (!__instance.truckSegment.underCreatureAttack && __instance.IsPowered())
                    {
                        if (__instance.CanTurn())
                        {
                            if (___animator)
                            {
                                ___animAccel = Mathf.Lerp(___animAccel, (float)__instance.leverDirection.y, Time.deltaTime * 3f);
                                ___animator.SetFloat("move_speed_z", ___animAccel);
                            }
                        }
                        if (__instance.upgrades && GameInput.GetButtonDown(GameInput.Button.Sprint))
                        {
                            __instance.upgrades.TryActivateAfterBurner();
                        }
                    }
                    if (___inputStackDummy.activeInHierarchy && IngameMenu.main != null)
                    {
                        if (GameInput.GetButtonDown(GameInput.Button.UIMenu))
                        {
                            IngameMenu.main.Open();
                        }
                        else if (!IngameMenu.main.gameObject.activeInHierarchy)
                        {
                            UWE.Utils.lockCursor = true;
                        }
                    }
                }
                if (__instance.engineSound)
                {
                    if (__instance.piloting && __instance.IsPowered())
                    {
                        __instance.engineSound.Play();
                        __instance.engineSound.SetParameterValue(___velocityParamIndex, __instance.useRigidbody.velocity.magnitude);
                        __instance.engineSound.SetParameterValue(___depthParamIndex, __instance.transform.root.position.y);
                        __instance.engineSound.SetParameterValue(___rpmParamIndex, (GameInput.GetMoveDirection().z + 1f) * 0.5f);
                        __instance.engineSound.SetParameterValue(___turnParamIndex, Mathf.Clamp(GameInput.GetLookDelta().x * 0.3f, -1f, 1f));
                        __instance.engineSound.SetParameterValue(___upgradeParamIndex, (float)(((__instance.powerEfficiencyFactor < 1f) ? 1 : 0) + (__instance.horsePowerUpgrade ? 2 : 0)));
                        if (__instance.liveMixin)
                        {
                            __instance.engineSound.SetParameterValue(___damagedParamIndex, 1f - __instance.liveMixin.GetHealthFraction());
                        }
                    }
                    else
                    {
                        __instance.engineSound.Stop();
                    }
                }
                if (___waitForDocking && !__instance.truckSegment.IsDocking())
                {
                    ___waitForDocking = false;
                    Player.main.ExitLockedMode(false, false, null);
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(MainCameraControl))]
    [HarmonyPatch("Update")]
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

            void cameraRelinquish()
            {
                mainCam.ResetCamera();
                mainCam.cinematicMode = false;
                mainCam.lookAroundMode = true;
                FreeLookPatcher.isFreeLooking = false;
            }

            if (Input.GetKeyDown(FreeLookPatcher.Config.FreeLookKey) || isTriggerNewlyDown)
            {
                Debug.Log("FreeLook: button pressed. Taking control of the camera.");
                FreeLookPatcher.isFreeLooking = true;
                isTriggerNewlyDown = false;

                resetCameraFlag = false;
                // invoke a camera vulnerability
                mainCam.cinematicMode = true;
                mainCam.lookAroundMode = false;
                return false;
            }
            else if (Input.GetKeyUp(FreeLookPatcher.Config.FreeLookKey) || isTriggerNewlyUp)
            {
                isTriggerNewlyUp = false;
                Debug.Log("FreeLook: button released. Relinquishing control of the camera.");
                resetCameraFlag = true;
            }
            if (!resetCameraFlag && (Input.GetKey(FreeLookPatcher.Config.FreeLookKey) || isTriggerDown))
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
                mainCam.rotationX = Mathf.SmoothDampAngle(mainCam.rotationX, 0f, ref xVelocity, smoothTime);
                mainCam.rotationY = Mathf.SmoothDampAngle(mainCam.rotationY, 0f, ref yVelocity, smoothTime);

                mainCam.camRotationX = mainCam.rotationX;
                mainCam.camRotationY = mainCam.rotationY;

                mainCam.transform.localEulerAngles = new Vector3(-mainCam.camRotationY, mainCam.camRotationX, 0);

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
