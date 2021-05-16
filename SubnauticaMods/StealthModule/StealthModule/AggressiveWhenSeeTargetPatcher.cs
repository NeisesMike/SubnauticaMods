using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace StealthModule
{
    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    [HarmonyPatch("IsTargetValid", typeof(GameObject))]
    class AggressiveWhenSeeTargetIsTargetValidPatcher
    {
		[HarmonyPrefix]
		public static bool Prefix(AggressiveWhenSeeTarget __instance, GameObject target)
		{
			return false;
		}

		[HarmonyPostfix]
        public static void Postfix(AggressiveWhenSeeTarget __instance, GameObject target, ref bool __result, TechType ___myTechType, Creature ___creature, bool ___ignoreSameKind,
			bool ___targetShouldBeInfected, float ___maxRangeScalar, float ___minimumVelocity)
        {
			string myTechType = ___myTechType.AsString(true);
			if (myTechType == "ghostleviathan" && __instance.lastTarget.target != null)
			{
				Logger.Log(__instance.lastTarget.target.ToString());
			}

			float myMaxRangeScalar = 10f;
			switch (StealthModulePatcher.Config.stealthQuality)
			{
				case (StealthQuality.Low):
					myMaxRangeScalar = 8f; 
					return;
				case (StealthQuality.Medium):
					myMaxRangeScalar = 6f;
					return;
				case (StealthQuality.High):
					myMaxRangeScalar = 4f;
					return;
				default:
					__result = true;
					return;
			}


			if (target == null)
			{
				__result = false;
				return;
			}
			if (target == ___creature.friend)
			{
				__result = false;
				return;
			}
			if (target == Player.main.gameObject && !Player.main.CanBeAttacked())
			{
				__result = false;
				return;
			}
			if (___ignoreSameKind && CraftData.GetTechType(target) == ___myTechType)
			{
				__result = false;
				return;
			}
			if (___targetShouldBeInfected)
			{
				InfectedMixin component = target.GetComponent<InfectedMixin>();
				if (component == null || component.GetInfectedAmount() < 0.33f)
				{
					__result = false;
					return;
				}
			}
			if (Vector3.Distance(target.transform.position, ___creature.transform.position) > myMaxRangeScalar)
			{
				__instance.lastTarget.target = null;
				   __result = false;
				return;
			}
			if (!Mathf.Approximately(___minimumVelocity, 0f))
			{
				Rigidbody componentInChildren = target.GetComponentInChildren<Rigidbody>();
				if (componentInChildren != null && componentInChildren.velocity.magnitude <= ___minimumVelocity)
				{
					__result = false;
					return;
				}
			}
			if(!___creature.GetCanSeeObject(target))
            {
				__result = false;
				return;
			}
        }
    }
}
