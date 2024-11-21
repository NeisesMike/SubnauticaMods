using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace StealthModule
{
    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("ChooseBestAction")]
    class CreaturePatcher
    {
		[HarmonyPostfix]
		public static void Postfix(Creature __instance, float time, ref CreatureAction __result, List<CreatureAction> ___actions, CreatureAction ___prevBestAction,
			int ___indexLastActionChecked)
		{
			// Ensure the player is in a vehicle with a stealth module equipped
			if(Player.main.currentMountedVehicle == null || Player.main.currentMountedVehicle.GetComponent<StealthModule>() == null)
            {
				return;
            }

			StealthQuality thisVehicleSQ = Player.main.currentMountedVehicle.GetComponent<StealthModule>().quality;

			float distToPlayer = Vector3.Distance(Player.main.transform.position, __instance.transform.position);

			// Determine the effectiveness of our module
			float myMaxRange;
			switch (thisVehicleSQ)
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

			if(thisVehicleSQ == StealthQuality.None)
            {
				return;
            }

			// report on nearby dangerous leviathans
			if (MainPatcher.config.isDistanceIndicatorEnabled && distToPlayer < 150)
			{
				if (__instance.name.Contains("GhostLeviathan"))
				{
					VehicleFramework.Logger.Output("Ghost Leviathan Distance: " + distToPlayer);
				}
				else if (__instance.name.Contains("ReaperLeviathan"))
				{
					VehicleFramework.Logger.Output("Reaper Leviathan Distance: " + distToPlayer);
				}
				else if (__instance.name.Contains("SeaDragon"))
				{
					VehicleFramework.Logger.Output("Sea Dragon Leviathan Distance: " + distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("gulper"))
				{
					VehicleFramework.Logger.Output("Gulper Distance: " + distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("bloop"))
				{
					VehicleFramework.Logger.Output("Bloop Distance: " + distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("blaza"))
				{
					VehicleFramework.Logger.Output("Blaza Distance: " + distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("silence"))
				{
					VehicleFramework.Logger.Output("Silence Distance: " + distToPlayer);
				}
				else if (__instance.name.ToLower().Contains("mrteeth"))
				{
					VehicleFramework.Logger.Output("MrTeeth Distance: " + distToPlayer);
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

				// check if this action is violent or aggressive
				string actionName1 = creatureAction.GetType().Name;
				// || actionName1 == "MoveTowardsTarget" 
				if (actionName1.Contains("Attack") || actionName1.Contains("Aggressive"))
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
									MainPatcher.logger.LogInfo(__instance.name + " is replacing " + creatureAction.GetType().Name + "(Player) with SwimRandom.");
									creatureAction = new SwimRandom();
								}
							}
						}
						else
						{
							MainPatcher.logger.LogInfo(__instance.name + " is replacing " + creatureAction.GetType().Name + " with SwimRandom.");
							creatureAction = new SwimRandom();
						}
					}
				}

				num = creatureAction.Evaluate(__instance, time);
			}
			___indexLastActionChecked++;
			if (___indexLastActionChecked >= ___actions.Count)
			{
				___indexLastActionChecked = 0;
			}

			CreatureAction creatureAction2 = ___actions[___indexLastActionChecked];

			// check if this action is violent
			string actionName2 = creatureAction2.GetType().Name;
			if (actionName2.Contains("Attack") || actionName2.Contains("Aggressive"))
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
								MainPatcher.logger.LogInfo(__instance.name + " is replacing " + creatureAction2.GetType().Name + "(Player) with SwimRandom (2)");
								creatureAction2 = new SwimRandom();
							}
						}
					}
					else
					{
						MainPatcher.logger.LogInfo(__instance.name + " is replacing " + creatureAction2.GetType().Name + " with SwimRandom (2)");
						creatureAction2 = new SwimRandom();
					}
				}
			}

			float num2 = creatureAction2.Evaluate(__instance, time);

			if (num2 > num && !global::Utils.NearlyEqual(num2, 0f, 1E-45f))
			{
				creatureAction = creatureAction2;
			}

			__result = creatureAction;
			return;
		}
	}
}
