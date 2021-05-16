using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace StealthModule
{
	[HarmonyPatch(typeof(AggressiveToPilotingVehicle))]
	[HarmonyPatch("UpdateAggression")]
	class AggressiveToPilotingVehiclePatcher
	{
		[HarmonyPrefix]
		public static bool Prefix(AggressiveToPilotingVehicle __instance, Creature ___creature)
		{
			Player main = Player.main;
			if (main == null || main.GetMode() != Player.Mode.LockedPiloting)
			{
				return false;
			}

			float myRange;
			switch (StealthModulePatcher.Config.stealthQuality)
			{
				case (StealthQuality.Low):
					myRange = 16.66f;
					break;
				case (StealthQuality.Medium):
					myRange = 13.33f;
					break;
				case (StealthQuality.High):
					myRange = 10f;
					break;
				default:
					myRange = 20f;
					break;
			}

			Vehicle vehicle = main.GetVehicle();
			if (vehicle == null || Vector3.Distance(vehicle.transform.position, ___creature.transform.position) > myRange)
			{
				__instance.lastTarget.target = null;
				return false;
			}
			__instance.lastTarget.target = vehicle.gameObject;
			__instance.creature.Aggression.Add(__instance.aggressionPerSecond * __instance.updateAggressionInterval);
			return false;
		}

	}
}
