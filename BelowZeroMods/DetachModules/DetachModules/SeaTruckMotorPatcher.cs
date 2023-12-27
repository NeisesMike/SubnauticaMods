using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Collections;
using UWE;

namespace SeatruckHotkeys
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("Update")]
    class SeaTruckMotorPatcher
    {
		public static SeaTruckSegment GetFirstSegment(SeaTruckSegment mySegment)
		{
			SeaTruckSegment result = null;
			SeaTruckSegment seaTruckSegment = mySegment;
			while (seaTruckSegment)
			{
				if (!seaTruckSegment.isFrontConnected)
				{
					result = seaTruckSegment;
					break;
				}
				seaTruckSegment = seaTruckSegment.frontConnection.GetConnection().truckSegment;
			}
			return result;
		}

		public static void exitDirectlyToWater(SeaTruckMotor inputMotor, ref bool piloting, ref bool ikenabled)
		{
			Vector3 value;
			bool flag3;
			bool flag2 = inputMotor.truckSegment.FindExitPoint(out value, out flag3, SeaTruckAnimation.Animation.ExitPilot);
			flag3 = (!inputMotor.truckSegment.IsWalkable() || flag3);
			if (!flag2)
			{
				ErrorMessage.AddError(Language.main.Get("ExitFailedNoSpace"));
			}
			else
			{
				Player.main.ExitLockedMode(false, false, null);
				inputMotor.truckSegment.Exit(new Vector3?(value), flag3);

				if (!flag3)
				{
					inputMotor.seatruckanimation.currentAnimation = SeaTruckAnimation.Animation.ExitPilot;
				}
				else
				{
					//Arms controller needs a trigger ID set
					//Player.main.armsController.SetTrigger("seatruck_exit");
					inputMotor.animator.SetTrigger("seatruck_exit");
				}
                piloting = false;
                Player.main.inSeatruckPilotingChair = false;
                uGUI.main.quickSlots.SetTarget(null);
                if (inputMotor.stopPilotSound)
                {
                    Utils.PlayFMODAsset(inputMotor.stopPilotSound, Player.main.transform, 20f);
                }
                ikenabled = false;
                Player.main.armsController.SetWorldIKTarget(null, null);
            }
        }


		[HarmonyPostfix]
        public static void Postfix(SeaTruckMotor __instance, ref bool ____piloting, ref bool ____ikenabled)
		{

			SeaTruckSegment mainSegment = GetFirstSegment(__instance.GetComponentInParent<SeaTruckSegment>());
			bool isOurMotor = Vector3.Distance(Player.main.transform.position, __instance.transform.position) < 2;

			if (!isOurMotor || !Player.main.IsPilotingSeatruck() )
            {
				return;
            }

			// detach hotkey
			if (SeatruckHotkeysPatcher.Config.isDetachEnabled && Input.GetKey(SeatruckHotkeysPatcher.Config.detachModulesKey) && mainSegment.isRearConnected)
			{
				SeaTruckAnimation oldAnimation = mainSegment.seatruckanimation;
				mainSegment.seatruckanimation = null;
				mainSegment.OnClickDetachLever(null);
				mainSegment.seatruckanimation = oldAnimation;
				Logger.Output("Modules Disconnected!"); 
			}

			if (__instance.waitForAnimation && __instance.seatruckanimation != null && __instance.seatruckanimation.currentAnimation > SeaTruckAnimation.Animation.Idle)
			{
				return;
			}

			// direct exit hotkey
			if (SeatruckHotkeysPatcher.Config.isDirectExitEnabled && Input.GetKey(SeatruckHotkeysPatcher.Config.directExitKey))
            {
				exitDirectlyToWater(__instance, ref ____piloting, ref ____ikenabled);
            }
		}
	}
}
