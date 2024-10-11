using HarmonyLib;
using VehicleFramework.VehicleComponents;
using UnityEngine;

namespace VFDrillArm
{
	[HarmonyPatch(typeof(Exosuit))]
	public static class ExosuitPatcher
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Exosuit.HasDrill))]
		public static void ExosuitHasDrillPostfix(Exosuit __instance, ref bool __result)
		{
			if (__instance.leftArm.GetGameObject().GetComponent<VFArm>() != null || __instance.rightArm.GetGameObject().GetComponent<VFArm>() != null)
			{
				__result = true;
			}
		}
	}
}
