﻿using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace StealthModule
{
    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("ChooseBestAction")]
    class CreaturePatcher
    {
		[HarmonyPostfix]
		public static void Postfix(Creature __instance, ref CreatureAction __result, List<CreatureAction> ___actions, CreatureAction ___prevBestAction,
			int ___indexLastActionChecked)
		{
			float distToPlayer = Vector3.Distance(Player.main.transform.position, __instance.transform.position);

			// Determine the effectiveness of our module
			float myMaxRange;
			switch (StealthModulePatcher.stealthQuality)
			{
				case (StealthQuality.None):
					myMaxRange = float.MaxValue;
					break;
				case (StealthQuality.Low):
					myMaxRange = 80f;
					break;
				case (StealthQuality.Medium):
					myMaxRange = 60f;
					break;
				case (StealthQuality.High):
					myMaxRange = 40f;
					break;
				case (StealthQuality.Debug):
					myMaxRange = float.MinValue;
					break;
				default:
					myMaxRange = float.MaxValue;
					break;
			}

			if(StealthModulePatcher.stealthQuality == StealthQuality.None)
            {
				return;
            }

			// report on nearby dangerous leviathans
			if (distToPlayer < 150)
			{
				if (__instance.name.Contains("GhostLeviathan"))
				{
					Logger.Output("Ghost Leviathan Distance: " + distToPlayer);
				}
				else if (__instance.name.Contains("ReaperLeviathan"))
				{
					Logger.Output("Reaper Leviathan Distance: " + distToPlayer);
				}
				else if (__instance.name.Contains("SeaDragon"))
				{
					Logger.Output("Sea Dragon Leviathan Distance: " + distToPlayer);
				}
			}

			if (___actions.Count == 0)
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
			if (___prevBestAction != null)
			{
				creatureAction = ___prevBestAction;

				// check if this action is violent
				string actionName1 = creatureAction.GetType().Name;
				// || actionName1 == "MoveTowardsTarget" 
				if (actionName1 == "AttackLastTarget" || actionName1 == "AttackCyclops" || actionName1 == "EMPAttack" || actionName1 == "MushroomAttack")
				{
					// check whether we're in range of the player
					if(distToPlayer < myMaxRange)
                    {
						// continue as usual
                    }
					else
					{
						// special case for AttackLastTarget... target could be not the player
						if (actionName1 == "AttackLastTarget")
						{
							if (((AttackLastTarget)creatureAction).lastTarget.target)
							{
								if (((AttackLastTarget)creatureAction).lastTarget.target.name == "Player")
								{
									Logger.Log(__instance.name + " is replacing " + creatureAction.GetType().Name + "(Player) with SwimRandom.");
									creatureAction = new SwimRandom();
								}
							}
						}
						else
						{
							Logger.Log(__instance.name + " is replacing " + creatureAction.GetType().Name + " with SwimRandom.");
							creatureAction = new SwimRandom();
						}
					}
				}

				num = creatureAction.Evaluate(__instance);
			}
			___indexLastActionChecked++;
			if (___indexLastActionChecked >= ___actions.Count)
			{
				___indexLastActionChecked = 0;
			}

			CreatureAction creatureAction2 = ___actions[___indexLastActionChecked];

			// check if this action is violent
			string actionName2 = creatureAction2.GetType().Name;
			if (actionName2 == "AttackLastTarget" || actionName2 == "AttackCyclops" || actionName2 == "EMPAttack" || actionName2 == "MushroomAttack")
			{
				// check whether we're in range of the player
				if (distToPlayer < myMaxRange)
				{
					// continue as usual
				}
				else
				{
					// special case for AttackLastTarget... target could be not the player
					if (actionName2 == "AttackLastTarget")
					{
						if (((AttackLastTarget)creatureAction2).lastTarget.target)
						{
							if (((AttackLastTarget)creatureAction2).lastTarget.target.name == "Player")
							{
								Logger.Log(__instance.name + " is replacing " + creatureAction2.GetType().Name + "(Player) with SwimRandom (2)");
								creatureAction2 = new SwimRandom();
							}
						}
					}
					else
					{
						Logger.Log(__instance.name + " is replacing " + creatureAction2.GetType().Name + " with SwimRandom (2)");
						creatureAction2 = new SwimRandom();
					}
				}
			}

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
