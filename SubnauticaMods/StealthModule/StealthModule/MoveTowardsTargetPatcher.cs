using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace StealthModule
{
    [HarmonyPatch(typeof(MoveTowardsTarget))]
    [HarmonyPatch("UpdateCurrentTarget")]
    class MoveTowardsTargetPatcher
    {




		[HarmonyPrefix]
        public static bool Prefix(MoveTowardsTarget __instance)
		{

			FieldInfo creatureField = AccessTools.DeclaredField(typeof(CreatureAction), "creature");

			Creature myCreature = AccessTools.FieldRefAccess<MoveTowardsTarget, Creature>(__instance, "creature");

			Logger.Log(myCreature.ToString());

			/*
			ProfilingUtils.BeginSample("UpdateCurrentTarget");
			if (EcoRegionManager.main != null && (Mathf.Approximately(__instance.requiredAggression, 0f) || __instance.creature.Aggression.Value >= __instance.requiredAggression))
			{
				IEcoTarget ecoTarget = EcoRegionManager.main.FindNearestTarget(__instance.targetType, ___creature.transform.position, __instance.isTargetValidFilter, 1);



				//Logger.Log(ecoTarget.GetGameObject().ToString());


				if (ecoTarget != null)
				{
					float distToPlayer = Vector3.Distance(Player.main.transform.position, ___creature.transform.position);
					float myMaxRange;
					switch (StealthModulePatcher.Config.stealthQuality)
					{
						case (StealthQuality.Low):
							myMaxRange = 150f;
							break;
						case (StealthQuality.Medium):
							myMaxRange = 100f;
							break;
						case (StealthQuality.High):
							myMaxRange = 75f;
							break;
						default:
							myMaxRange = float.MaxValue;
							break;
					}

					// if target is player and distance isn't right, get new target

					__instance.currentTarget = ecoTarget;
				}
				else
				{
					__instance.currentTarget = null;
				}
			}
			ProfilingUtils.EndSample(null);
			*/

			return true;
        }
    }
}
