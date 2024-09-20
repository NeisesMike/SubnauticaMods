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

namespace PassiveReaperLeviathans
{
	/*
	 * This works by removing aggressive actions from the action list before iterating through the action list to find the best possible action.
	 * ChooseBestAction is the function of interest. The surrounding function is largely the same.
	 * 
	 */
	/*
	[HarmonyPatch(typeof(Creature))]
	[HarmonyPatch("UpdateBehaviour")]
	class CreatureUpdateBehaviourPatcher
	{
		[HarmonyPrefix]
		public static bool Prefix(Creature __instance, float time, float deltaTime, Animator ___traitsAnimator, ref CreatureAction ___lastAction,
			CreatureAction ___prevBestAction, List<CreatureAction> ___actions, ref int ___indexLastActionChecked,
			ref int ___animAggressive, ref int ___animScared, ref int ___animTired, ref int ___animHappy)
		{
			TechType techType = CraftData.GetTechType(__instance.gameObject);
			string myTechType = techType.AsString(true);

			bool isThisCaseSomethingWeCareAbout = myTechType == "reaperleviathan" && MainPatcher.config.isReaperPassive;
			if (!isThisCaseSomethingWeCareAbout)
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

				float mynum = 0f;
				CreatureAction CBAAction1 = null;
				if (___prevBestAction != null)
				{
					CBAAction1 = ___prevBestAction;
					mynum = CBAAction1.Evaluate(__instance, time);
				}

				myIndexLast++;
				if (myIndexLast >= ___actions.Count)
				{
					// This condition is always taken.
					// After ScanActions or whatever, the ___indexLastActionChecked is set to the index of the final action.
					// So when we increment it, it's literally always going to be greater than or equal to the actions.count
					myIndexLast = 0;
				}

				/*
				*(SwimRandom)
				*(StayAtLeashPosition)
				*(FleeOnDamage)
				*(AttackLastTarget)
				*(AttackCyclops)
				*(AvoidTerrain)
				 */
	/*
				int num2 = 0;
				List<CreatureAction> passive_actions = new List<CreatureAction>();
				passive_actions.AddRange(___actions);
				passive_actions.RemoveAll(t => t.ToString().Contains("AttackLastTarget") || t.ToString().Contains("AttackCyclops"));

				for (int i = 0; i < passive_actions.Count; i++)
				{
					CreatureAction iterCreatureAction = passive_actions[i];
					if (!(iterCreatureAction == ___prevBestAction) && (i == num2 || iterCreatureAction.NeedsToBeChecked(time)))
					{
						iterCreatureAction.timeLastChecked = time;
						if (iterCreatureAction.GetMaxEvaluatePriority() > mynum)
						{
							float num3 = iterCreatureAction.Evaluate(__instance, time);
							if (num3 > mynum)
							{
								mynum = num3;
								CBAAction1 = iterCreatureAction;
							}
						}
					}
				}
				return CBAAction1;
			}

			___indexLastActionChecked = myIndexLast;

			CreatureAction creatureAction = ChooseBestAction();
			if (___prevBestAction != creatureAction)
			{
				if (___prevBestAction)
				{
					___prevBestAction.StopPerform(__instance, time);
				}
				if (creatureAction)
				{
					creatureAction.StartPerform(__instance, time);
				}
				___prevBestAction = creatureAction;
			}
			if (creatureAction)
			{
				creatureAction.Perform(__instance, time, deltaTime);
			}
			float num = DayNightUtils.Evaluate(1f, __instance.activity);
			__instance.Tired.Value = Mathf.Lerp(__instance.Tired.Value, 1f - num, 0.1f * deltaTime);
			__instance.Curiosity.UpdateTrait(deltaTime);
			__instance.Friendliness.UpdateTrait(deltaTime);
			__instance.Hunger.UpdateTrait(deltaTime);
			__instance.Aggression.UpdateTrait(deltaTime);
			__instance.Scared.UpdateTrait(deltaTime);
			__instance.Tired.UpdateTrait(deltaTime);
			__instance.Happy.UpdateTrait(deltaTime);
			if (___traitsAnimator && ___traitsAnimator.isActiveAndEnabled)
			{
				___traitsAnimator.SetFloat(___animAggressive, __instance.Aggression.Value);
				___traitsAnimator.SetFloat(___animScared, __instance.Scared.Value);
				___traitsAnimator.SetFloat(___animTired, __instance.Tired.Value);
				___traitsAnimator.SetFloat(___animHappy, __instance.Happy.Value);
			}
			return false;
		}
	}
*/
}
