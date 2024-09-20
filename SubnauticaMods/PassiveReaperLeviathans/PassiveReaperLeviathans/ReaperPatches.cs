using UnityEngine;
using HarmonyLib;

namespace PassiveReaperLeviathans
{
	[HarmonyPatch(typeof(AttackCyclops))]
	class AttackCyclopsPatches
	{
		[HarmonyPatch("OnCollisionEnter")]
		[HarmonyPrefix]
		public static bool OnCollisionEnterPrefix(AttackCyclops __instance, Collision collision)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			bool shouldWeManageAdult = techType is TechType.ReaperLeviathan && MainPatcher.config.ignoreCyclops;
			if (shouldWeManageAdult && collision?.gameObject?.GetComponent<CyclopsNoiseManager>())
			{
				// Don't get mad when we bump into a cyclops
				return false;
			}
			return true;
		}

		[HarmonyPatch("SetCurrentTarget")]
		[HarmonyPrefix]
		public static bool SetCurrentTargetPrefix(AttackCyclops __instance, GameObject target, bool isDecoy)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			bool shouldWeManageAdult = (techType is TechType.ReaperLeviathan) && MainPatcher.config.ignoreCyclops;
			if (shouldWeManageAdult && target?.GetComponent<CyclopsNoiseManager>())
			{
				// When we would target a cyclops, instead don't
				return false;
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(LastTarget))]
	class LastTargetPatches
	{
		[HarmonyPatch("SetTargetInternal")]
		[HarmonyPrefix]
		public static bool SetTargetInternalPrefix(LastTarget __instance, GameObject newTarget)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.ReaperLeviathan)
			{
				bool ignorePlayer = MainPatcher.config.ignorePlayer && newTarget?.GetComponent<Player>() != null;
				bool ignoreCyclops = MainPatcher.config.ignoreCyclops && newTarget?.GetComponent<CyclopsNoiseManager>() != null;
				bool ignoreVehicle = MainPatcher.config.ignoreVehicles && newTarget?.GetComponent<Vehicle>() != null;
				if (ignorePlayer || ignoreCyclops || ignoreVehicle)
				{
					return false;
				}
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(ReaperMeleeAttack))]
	class ReaperMeleeAttackPatches
	{
		[HarmonyPatch("OnTouch")]
		[HarmonyPrefix]
		public static bool SetTargetInternalPrefix(ReaperMeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.ReaperLeviathan && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoBiteDamage)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(MeleeAttack))]
	class MeleeAttackPatches
	{
		[HarmonyPatch("CanDealDamageTo")]
		[HarmonyPrefix]
		public static bool CanDealDamageToPrefix(MeleeAttack __instance, GameObject target, ref bool __result)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.ReaperLeviathan && (target.GetComponent<Player>() || target.GetComponent<Vehicle>() || target.GetComponent<SubControl>()) && MainPatcher.config.isNoBiteDamage)
			{
				__result = false;
				return false;
			}
			return true;
		}
		[HarmonyPatch("OnTouch")]
		[HarmonyPrefix]
		public static bool SetTargetInternalPrefix(ReaperMeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.ReaperLeviathan && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoBiteDamage)
			{
				return false;
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(ReaperLeviathan))]
	class ReaperLeviathanPatches
	{
		[HarmonyPatch("GetCanGrabVehicle")]
		[HarmonyPostfix]
		public static void SetTargetInternalPrefix(ReaperLeviathan __instance, ref bool __result)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.ReaperLeviathan && MainPatcher.config.ignoreVehicles)
			{
				__result = false;
			}
		}
	}

	[HarmonyPatch(typeof(OnTouch))]
	class OnTouchPatches
	{
		[HarmonyPatch("OnTriggerEnter")]
		[HarmonyPrefix]
		public static bool SetTargetInternalPrefix(OnTouch __instance, Collider collider)
		{
			if (__instance.GetComponent<ReaperLeviathan>() && __instance.name=="mouth_damage_trigger" && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoBiteDamage)
			{
				return false;
			}
			return true;
		}
	}



	
}
