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
		[HarmonyPostfix]
        public static void Postfix(SeaTruckMotor __instance, ref bool ____piloting, ref bool ____ikenabled)
		{
			Tuple<bool,bool> directives = HandleHotkeys();
			SeaTruckSegment mainSegment = GetFirstSegment(__instance.GetComponentInParent<SeaTruckSegment>());
			bool isOurMotor = Vector3.Distance(Player.main.transform.position, __instance.transform.position) < 2;
			if (!isOurMotor || !Player.main.IsPilotingSeatruck() )
            {
				return;
            }
			if (__instance.waitForAnimation && __instance.seatruckanimation != null && __instance.seatruckanimation.currentAnimation > SeaTruckAnimation.Animation.Idle)
			{
				return;
			}

			// direct exit hotkey
			if (SeatruckHotkeysPatcher.SHConfig.isDirectExitEnabled && directives.Item1)
			{
				exitDirectlyToWater(__instance, ref ____piloting, ref ____ikenabled);
			}

			// detach hotkey
			if (SeatruckHotkeysPatcher.SHConfig.isDetachEnabled && directives.Item2 && mainSegment.isRearConnected)
			{
				SeaTruckAnimation oldAnimation = mainSegment.seatruckanimation;
				mainSegment.seatruckanimation = null;
				mainSegment.OnClickDetachLever(null);
				mainSegment.seatruckanimation = oldAnimation;
				Logger.Output("Modules Disconnected!");
			}
		}

		public static Tuple<bool, bool> HandleHotkeys()
		{
			bool shouldDirectExit = Input.GetKeyDown(SeatruckHotkeysPatcher.SHConfig.directExitKey);
			bool shouldDetach = Input.GetKeyDown(SeatruckHotkeysPatcher.SHConfig.detachModulesKey);
			return new Tuple<bool, bool>(shouldDirectExit, shouldDetach);
		}

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
			Vector3 exitPosition;
			bool willSkipAnimation;
			bool foundExitPoint = inputMotor.truckSegment.FindExitPoint(out exitPosition, out willSkipAnimation, SeaTruckAnimation.Animation.ExitPilot);
			if (!foundExitPoint)
			{
				ErrorMessage.AddError(Language.main.Get("ExitFailedNoSpace"));
				return;
			}
			willSkipAnimation = (!inputMotor.truckSegment.IsWalkable() || willSkipAnimation);
			Player.main.ExitLockedMode(false, false, null);

			if (!foundExitPoint)
			{
				ErrorMessage.AddError(Language.main.Get("ExitFailedNoSpace"));
				inputMotor.truckSegment.Exit(null, false, false, null, null);
			}
			else
			{
				inputMotor.truckSegment.Exit(new Vector3?(exitPosition), skipAnimations: willSkipAnimation);
			}
			if (!willSkipAnimation)
			{
				inputMotor.seatruckanimation.currentAnimation = SeaTruckAnimation.Animation.ExitPilot;
			}
			else
			{
				Player.main.armsController.SetTrigger(AnimatorHashID.seatruck_exit);
				inputMotor.animator.SetTrigger("seatruck_exit");
			}

			piloting = false;
			inputMotor.UpdateIKEnabledState();
			Player.main.inSeatruckPilotingChair = false;
			uGUI.main.quickSlots.SetTarget(null);
			if (inputMotor.stopPilotSound)
			{
				Utils.PlayFMODAsset(inputMotor.stopPilotSound, Player.main.transform, 20f);
			}
			inputMotor.UpdateIKEnabledState();
			//ikenabled = false;
			Player.main.armsController.SetWorldIKTarget(null, null);
		}
	}
}
