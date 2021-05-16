using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace StealthModule
{
    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("ChooseBestAction")]
    class CreaturePatcher
    {
		[HarmonyPrefix]
		public static bool Prefix(Creature __instance)
		{
			return false;
		}

		[HarmonyPostfix]
		public static void Postfix(Creature __instance, ref CreatureAction __result)
		{
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



			if (__instance.actions.Count == 0)
			{
				__result = null;
				return;
			}
			if (__instance.liveMixin && !__instance.liveMixin.IsAlive())
			{
				SwimBehaviour component = __instance.transform.root.GetComponent<SwimBehaviour>();
				if (component)
				{
					component.Idle();
				}
				__result = null;
				return;
			}
			float num = 0f;
			CreatureAction creatureAction = null;
			if (__instance.prevBestAction != null)
			{
				creatureAction = __instance.prevBestAction;
				num = creatureAction.Evaluate(__instance);
			}
			__instance.indexLastActionChecked++;
			if (__instance.indexLastActionChecked >= __instance.actions.Count)
			{
				__instance.indexLastActionChecked = 0;
			}
			CreatureAction creatureAction2 = __instance.actions[__instance.indexLastActionChecked];
			float num2 = creatureAction2.Evaluate(__instance);

			if (num2 > num && !global::Utils.NearlyEqual(num2, 0f, 1E-45f))
			{
				creatureAction = creatureAction2;
			}

			__result = creatureAction;
			return;
		}
	}
}
