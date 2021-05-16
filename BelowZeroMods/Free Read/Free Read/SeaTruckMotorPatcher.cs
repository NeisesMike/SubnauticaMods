using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;

namespace FreeRead
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeRead] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[FreeRead] " + string.Format(format, args));
        }
    }
    public class FreeReadPatcher
    {
        public static bool isCruising = false;
        public static bool isInPDA = false;

        public static void Patch()
        {
            var harmony = new Harmony("com.garyburke.subnautica.freeread.mod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("FixedUpdate")]
    public class SeaTruckMotorFixedUpdatePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, bool ____piloting)
        {
            /*
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                    Logger.Log("KeyCode down: " + kcode);
            }
            */

            bool isThisOurTruck = Vector3.Distance(Player.main.transform.position, __instance.transform.position) < 2;
            bool isAutoMoveClicked = (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.JoystickButton8));

            if(!isThisOurTruck)
            {
                return true;
            }

            if (FreeReadPatcher.isInPDA && isAutoMoveClicked)
            {
                Logger.Log("Toggling cruise");
                FreeReadPatcher.isCruising = !FreeReadPatcher.isCruising;
            }

            if (FreeReadPatcher.isCruising)
            {
                Logger.Log("...cruising...");

                // if keyboard, allow translation ( f and b cancel auto-move )

                // if controller, allow rotation



                //=====================================
                // the rest of this is base game code
                //=====================================
                bool myIsPowered = !__instance.requiresPower || (__instance.relay && __instance.relay.IsPowered());
                bool myIsBusyAnimating = __instance.waitForAnimation && __instance.seatruckanimation != null && __instance.seatruckanimation.currentAnimation > SeaTruckAnimation.Animation.Idle;
                if (Player.main.transform.position.y < Ocean.GetOceanLevel() && __instance.useRigidbody != null && myIsPowered && !myIsBusyAnimating)
			    {
                    // "vector" as described by GameInput.UpdateMoveDirection
                    Vector3 vector = new Vector3(0, 0, 1);
					Vector3 a = MainCameraControl.main.rotation * vector;
                    float myGetWeight = __instance.truckSegment.GetWeight() + __instance.truckSegment.GetAttachedWeight() * (__instance.horsePowerUpgrade ? 0.65f : 0.8f);
                    float num = 1f / Mathf.Max(1f, myGetWeight * 0.35f) * __instance.acceleration;
					__instance.useRigidbody.AddForce(num * a, ForceMode.Acceleration);
					if (__instance.relay)
					{
						float num2;
						__instance.relay.ConsumeEnergy(Time.deltaTime * __instance.powerEfficiencyFactor * 0.12f, out num2);
					}
                    return false;
				}
			}
            return true;
		}
    }
}
