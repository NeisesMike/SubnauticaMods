using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace StealthModule
{
	[HarmonyPatch(typeof(Creature))]
	class CreaturePatcher
	{
		private static readonly List<string> violentActionNames = new List<string>() { "Attack", "Aggressive", "Grab"};
		public static void Output(Creature creat, string message, float distance)
		{
			var marty = Player.main.GetComponent<StealthModuleLogger>();
			marty.Add(creat, message, distance);
		}
		public static StealthQuality CheckHasStealth()
		{
			StealthQuality thisVehicleSQ = StealthQuality.None;
			if (Player.main.currentMountedVehicle != null)
			{
				if (Player.main.currentMountedVehicle.GetComponent<StealthModule>() != null)
				{
					thisVehicleSQ = Player.main.currentMountedVehicle.GetComponent<StealthModule>().quality;
				}
			}
			else if (Player.main.currentSub != null)
			{
				if (Player.main.currentSub.GetComponent<StealthModule>() != null)
				{
					thisVehicleSQ = Player.main.currentSub.GetComponent<StealthModule>().quality;
				}
			}
			return thisVehicleSQ;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Creature.ScheduledUpdate))]
		public static void CreatureScheduledUpdateHarmonyPostfix(Creature __instance)
		{
			StealthQuality thisVehicleSQ = CheckHasStealth();
			if (thisVehicleSQ == StealthQuality.None)
			{
				return;
			}

			// report on nearby dangerous leviathans
			float distToPlayer = Vector3.Distance(Player.main.transform.position, __instance.transform.position);
			if (MainPatcher.config.isDistanceIndicatorEnabled && distToPlayer < 150)
			{
				if (__instance.name.Contains("GhostLeviathan"))
				{
					Output(__instance, "Ghost Leviathan: ", distToPlayer);
				}
				else if (__instance.name.Contains("ReaperLeviathan"))
				{
					Output(__instance, "Reaper Leviathan: ", distToPlayer);
				}
				else if (__instance.name.Contains("SeaDragon"))
				{
					Output(__instance, "Sea Dragon Leviathan: ", distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("gulper"))
				{
					Output(__instance, "Gulper: ", distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("bloop"))
				{
					Output(__instance, "Bloop: ", distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("blaza"))
				{
					Output(__instance, "Blaza: ", distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("silence"))
				{
					Output(__instance, "Silence: ", distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("mrteeth"))
				{
					Output(__instance, "MrTeeth: ", distToPlayer);
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Creature.ChooseBestAction))]
		public static void CreatureChooseBestActionPostfix(Creature __instance, ref CreatureAction __result)
		{
			if (__result == null || __instance == null) return;

			string thisActionName = __result.GetType().ToString();
			bool isActionNonViolent = true;
			foreach (string actionName in violentActionNames)
			{
				if (thisActionName.Contains(actionName))
				{
					isActionNonViolent = false;
					break;
				}
			}

			if (isActionNonViolent) return;

			// Assume that LastTarget is at the root of the game object
			LastTarget lastTarget = __instance.GetComponent<LastTarget>();
			if (lastTarget == null || lastTarget.target == null) return;

			StealthModule targetSM = lastTarget.target.GetComponent<StealthModule>();
			if (targetSM == null) return;

			if (targetSM.quality == StealthQuality.None) return;

			float myMaxRange = StealthModule.GetMaxRange(targetSM.quality);
			float distToTarget = Vector3.Distance(lastTarget.target.transform.position, __instance.transform.position);

			if (distToTarget < myMaxRange) return; // if the creature is too close to the vehicle, we can't do any stealth.

            if (MainPatcher.config.isEffectLogging)
            {
				MainPatcher.logger.LogInfo($"Creature {__instance.name} is replacing action {thisActionName} against target {lastTarget.target.name} with action SwimRandom");
			}
			__result = new SwimRandom();
		}
	}
}
