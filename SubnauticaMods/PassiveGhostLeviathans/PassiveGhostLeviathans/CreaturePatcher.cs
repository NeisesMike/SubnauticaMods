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

namespace PassiveGhostLeviathans
{
	/*
	 * I honestly have no recall of how this works.
	 * I thought it was simply setting aggression to zero, but no
	 * Then I thought it was replacing aggressive actions with non-aggressive actions, but that doesn't seem to be it
	 * Does this work?
	 * 
	 */
	[HarmonyPatch(typeof(Creature))]
	[HarmonyPatch("UpdateBehaviour")]
	class CreatureUpdateBehaviourPatcher
	{
		[HarmonyPrefix]
		public static bool Prefix(Creature __instance, float deltaTime, Animator ___traitsAnimator, ref CreatureAction ___lastAction,
			CreatureAction ___prevBestAction, float ___flinch, float ___flinchFadeRate, List<CreatureAction> ___actions, ref int ___indexLastActionChecked,
			ref int ___animAggressive, ref int ___animScared, ref int ___animTired, ref int ___animHappy, ref int ___animFlinch)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			string myTechType = techType.AsString(true);

			bool shouldWeManageAdult = myTechType == "ghostleviathan" && PassiveGhostLeviathansPatcher.Config.isGhostPassive;
			bool shouldWeManageJuvenile = myTechType == "ghostleviathanjuvenile" && PassiveGhostLeviathansPatcher.Config.isJuvenileGhostPassive;
			bool isThisCaseSomethingWeCareAbout = shouldWeManageAdult || shouldWeManageJuvenile;

			if(!isThisCaseSomethingWeCareAbout)
            {
				return true;
            }

			int myIndexLast = ___indexLastActionChecked;

			CreatureAction ChooseBestAction()
			{
				if (___actions.Count == 0)
				{
					return null;
				}
				if (__instance.liveMixin && !__instance.liveMixin.IsAlive())
				{
					SwimBehaviour component = __instance.GetComponentInParent<SwimBehaviour>();
					if (component)
					{
						component.Idle();
					}
					return null;
				}
				ProfilingUtils.BeginSample("Creature.ChooseBestAction");
				float mynum = 0f;
				CreatureAction mycreatureAction = null;
				if (___prevBestAction != null)
				{
					mycreatureAction = ___prevBestAction;
					ProfilingUtils.BeginSample("bestAction.Evaluate");
					mynum = mycreatureAction.Evaluate(__instance);
					ProfilingUtils.EndSample(null);
				}
				myIndexLast++;
				if (myIndexLast >= ___actions.Count)
				{
					myIndexLast = 0;
				}
				CreatureAction creatureAction2 = ___actions[myIndexLast];
				ProfilingUtils.BeginSample("current.Evaluate");
				float num2 = creatureAction2.Evaluate(__instance);
				ProfilingUtils.EndSample(null);
				if (num2 > mynum && !Utils.NearlyEqual(num2, 0f, 1E-45f))
				{
					mycreatureAction = creatureAction2;
				}
				ProfilingUtils.EndSample(null);
				return mycreatureAction;
			}

			___indexLastActionChecked = myIndexLast;

			ProfilingUtils.BeginSample("CreatureBehavior::Update");
			ProfilingUtils.BeginSample("choose action");
			CreatureAction creatureAction = ChooseBestAction();
			if (___prevBestAction != creatureAction)
			{
				if (___prevBestAction)
				{
					___prevBestAction.StopPerform(__instance);
				}
				if (creatureAction)
				{
					//Logger.Log(creatureAction.ToString());
					creatureAction.StartPerform(__instance);
				}
				___prevBestAction = creatureAction;
			}
			ProfilingUtils.EndSample(null);
			if (creatureAction)
			{
				ProfilingUtils.BeginSample("perform action");
				creatureAction.Perform(__instance, deltaTime);
				ProfilingUtils.EndSample(null);
			}
			float num = DayNightUtils.Evaluate(1f, __instance.activity);
			__instance.Tired.Value = Mathf.Lerp(__instance.Tired.Value, 1f - num, 0.1f * deltaTime);
			ProfilingUtils.BeginSample("update traits");
			__instance.Curiosity.UpdateTrait(deltaTime);
			__instance.Friendliness.UpdateTrait(deltaTime);
			__instance.Hunger.UpdateTrait(deltaTime);
			__instance.Aggression.UpdateTrait(deltaTime);
			__instance.Scared.UpdateTrait(deltaTime);
			__instance.Tired.UpdateTrait(deltaTime);
			__instance.Happy.UpdateTrait(deltaTime);
			___flinch = Mathf.Lerp(___flinch, 0f, ___flinchFadeRate * deltaTime);
			ProfilingUtils.EndSample(null);
			if (___traitsAnimator && ___traitsAnimator.isActiveAndEnabled)
			{
				___traitsAnimator.SetFloat(___animAggressive, __instance.Aggression.Value);
				___traitsAnimator.SetFloat(___animScared, __instance.Scared.Value);
				___traitsAnimator.SetFloat(___animTired, __instance.Tired.Value);
				___traitsAnimator.SetFloat(___animHappy, __instance.Happy.Value);
				___traitsAnimator.SetFloat(___animFlinch, ___flinch);
			}
			ProfilingUtils.EndSample(null);
			return false;
		}
	}

	[HarmonyPatch(typeof(GhostLeviathanMeleeAttack))]
	[HarmonyPatch("GetBiteDamage")]
	class GhostLeviathanMeleeAttackGetBiteDamagePatcher
	{
		[HarmonyPostfix]
		public static void Postfix(GhostLeviathanMeleeAttack __instance, ref float __result)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			string myTechType = techType.AsString(true);

			bool shouldWeManageAdult = myTechType == "ghostleviathan" && PassiveGhostLeviathansPatcher.Config.isNoBiteDamage;
			bool shouldWeManageJuvenile = myTechType == "ghostleviathanjuvenile" && PassiveGhostLeviathansPatcher.Config.isJuvenileNoBiteDamage;

			if(shouldWeManageAdult || shouldWeManageJuvenile)
            {
				__result = 0;
            }
		}
	}
}
