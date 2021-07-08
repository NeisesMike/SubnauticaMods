using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;


namespace PersistentReaper
{
	// Ensure that if Percy is locked onto the player,
	// he moves in for the attack
	[HarmonyPatch(typeof(Creature))]
	[HarmonyPatch("ChooseBestAction")]
	class CreaturePatcher
	{
		public static CreatureAction attackPlayer(GameObject percy)
		{
			if (Player.main.inSeamoth)
			{
				return percy.GetComponent<AttackCyclops>();
			}
			else if (Player.main.inExosuit)
			{
				return percy.GetComponent<AttackCyclops>();
			}
			else if (Player.main.currentSub)
			{
				//percy.GetComponent<AttackCyclops>().lastTarget.target = Player.main.gameObject;
				return percy.GetComponent<AttackCyclops>();
			}
			else
			{
				//percy.GetComponent<AttackLastTarget>().lastTarget.target = Player.main.gameObject;
				return percy.GetComponent<AttackLastTarget>();
			}
		}

		[HarmonyPostfix]
		public static void Postfix(Creature __instance, ref CreatureAction __result, CreatureAction ___prevBestAction)
		{
			// ensure we're Percy and locked onto the player
			if (!(ReaperManager.reaperDict.ContainsKey(__instance.gameObject) && ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer))
			{
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

			CreatureAction creatureAction = attackPlayer(__instance.gameObject);

			if (___prevBestAction != null)
			{
				creatureAction = ___prevBestAction;
				num = creatureAction.Evaluate(__instance);
			}

			CreatureAction creatureAction2 = creatureAction;

			float num2 = creatureAction2.Evaluate(__instance);

			if (num2 > num && !global::Utils.NearlyEqual(num2, 0f, 1E-45f))
			{
				creatureAction = creatureAction2;
			}

			__result = creatureAction;
			__instance.Hunger.UpdateTrait(1);
			__instance.Aggression.UpdateTrait(1);
			return;
		}
	}
}
