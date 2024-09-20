using UnityEngine;
using HarmonyLib;

namespace PassiveSeaDragons
{
	[HarmonyPatch(typeof(AttackCyclops))]
	class AttackCyclopsPatches
	{
		[HarmonyPatch("OnCollisionEnter")]
		[HarmonyPrefix]
		public static bool OnCollisionEnterPrefix(AttackCyclops __instance, Collision collision)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			bool shouldWeManageAdult = techType is TechType.SeaDragon && MainPatcher.config.ignoreCyclops;
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
			bool shouldWeManageAdult = (techType is TechType.SeaDragon) && MainPatcher.config.ignoreCyclops;
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
			if (techType is TechType.SeaDragon)
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

	[HarmonyPatch(typeof(AttackLastTarget))]
	class AttackLastTargetPatches
	{
		[HarmonyPatch("CanAttackTarget")]
		[HarmonyPostfix]
		public static void CanAttackTargetPostfix(SeaDragon __instance, GameObject target, ref bool __result)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);

			bool isSeaDragon = __instance.GetComponent<SeaDragon>();
			if(!isSeaDragon)
            {
				return;
            }
			
			bool ignorePlayer = MainPatcher.config.ignorePlayer && target?.GetComponent<Player>() != null;
			bool ignoreCyclops = MainPatcher.config.ignoreCyclops && target?.GetComponent<SubControl>() != null;
			bool ignoreVehicle = MainPatcher.config.ignoreVehicles && target?.GetComponent<Vehicle>() != null;
			bool ignoreDamage = MainPatcher.config.isNoDamage && (target?.GetComponent<Player>() || target?.GetComponent<SubControl>() || target?.GetComponent<Vehicle>());
			bool isProtectedObject = ignorePlayer || ignoreCyclops || ignoreVehicle || ignoreDamage;

			if (techType is TechType.SeaDragon && isProtectedObject)
			{
				__result = false;
			}
		}
	}


	[HarmonyPatch(typeof(SeaDragonMeleeAttack))]
	class SeaDragonMeleeAttackPatches
	{
		[HarmonyPatch("OnTouchFront")]
		[HarmonyPrefix]
		public static bool OnTouchFrontPrefix(SeaDragonMeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.SeaDragon && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoDamage)
			{
				return false;
			}
			return true;
		}
		[HarmonyPatch("OnTouchLeft")]
		[HarmonyPrefix]
		public static bool OnTouchLeftPrefix(SeaDragonMeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.SeaDragon && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoDamage)
			{
				return false;
			}
			return true;
		}
		[HarmonyPatch("OnTouchRight")]
		[HarmonyPrefix]
		public static bool OnTouchRightPrefix(SeaDragonMeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.SeaDragon && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoDamage)
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
			if (techType is TechType.SeaDragon && (target.GetComponent<Player>() || target.GetComponent<Vehicle>() || target.GetComponent<SubControl>()) && MainPatcher.config.isNoDamage)
			{
				__result = false;
				return false;
			}
			return true;
		}
		[HarmonyPatch("OnTouch")]
		[HarmonyPrefix]
		public static bool OnTouchPrefix(MeleeAttack __instance, Collider collider)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.SeaDragon && (collider.GetComponent<Player>() || collider.GetComponent<Vehicle>() || collider.GetComponent<SubControl>()) && MainPatcher.config.isNoDamage)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SeaDragon))]
	class SeaDragonPatches
	{
		[HarmonyPatch("GetCanGrabExosuit")]
		[HarmonyPostfix]
		public static void GetCanGrabExosuitPostfix(SeaDragon __instance, ref bool __result)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			if (techType is TechType.SeaDragon && (MainPatcher.config.ignoreVehicles || MainPatcher.config.isNoDamage))
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
		public static bool OnTriggerEnterPrefix(OnTouch __instance, Collider collider)
		{
			bool isSeaDragon = __instance.GetComponent<SeaDragon>();
			bool isSwatTrigger =  __instance.name.Contains("SwatAttackTrigger");
			bool isBiteTrigger = __instance.name == "mouth_damage_trigger";

			bool ignorePlayer = MainPatcher.config.ignorePlayer && collider.GetComponent<Player>() != null;
			bool ignoreCyclops = MainPatcher.config.ignoreCyclops && collider.GetComponent<SubControl>() != null;
			bool ignoreVehicle = MainPatcher.config.ignoreVehicles && collider.GetComponent<Vehicle>() != null;
			bool ignoreDamage = MainPatcher.config.isNoDamage && (collider.GetComponent<Player>() || collider.GetComponent<SubControl>() || collider.GetComponent<Vehicle>());
			bool isProtectedObject = ignorePlayer || ignoreCyclops || ignoreVehicle || ignoreDamage;

			if (isSeaDragon && (isSwatTrigger || isBiteTrigger) && isProtectedObject)
			{
				return false;
			}
			return true;
		}
	}




}
