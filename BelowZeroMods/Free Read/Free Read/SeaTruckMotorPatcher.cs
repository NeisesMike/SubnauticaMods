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
using UWE;

namespace FreeRead
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("FixedUpdate")]
    public class SeaTruckMotorFixedUpdatePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, bool ____piloting)
        {
            bool isThisOurTruck = Vector3.Distance(Player.main.transform.position, __instance.transform.position) < 2;
            bool isAutoMoveClicked = (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.JoystickButton8));

            if(!isThisOurTruck)
            {
                return true;
            }

            if (FreeReadPatcher.isInPDA && isAutoMoveClicked)
            {
                FreeReadPatcher.isCruising = !FreeReadPatcher.isCruising;
            }

            if (FreeReadPatcher.isCruising)
            {
                if (FreeReadPatcher.Config.isPDAPauseSuperceded)
                {
                    FreeReadOptions.isAllowingPause = false;
                }
                else
                {
                    FreeReadOptions.isAllowingPause = true;
                }

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
            else
            {
                FreeReadOptions.isAllowingPause = true;
            }
            return true;
		}
    }
}
