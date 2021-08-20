using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PersistentCreatures
{
	[HarmonyPatch(typeof(Creature))]
	public class CreaturePatcher
	{
		/*
		 * This is a bit of a hack,
		 * but it's not so bad
		 * If a banned creature tries to wake up, kill it immediately.
		 * Find some other way to prevent spawning banned creatures.
		 */
		[HarmonyPostfix]
		[HarmonyPatch("Start")]
		public static void StartPostfix(Creature __instance, List<CreatureAction> ___actions)
		{
			// If we've got no object, skip
			if(__instance.gameObject == null)
            {
				return;
            }
			// If we're a persistent creature, halt all actions
			if (__instance.gameObject.GetComponent<PersistentCreatureBehavior>() != null)
			{
				foreach (CreatureAction ca in ___actions)
				{
					ca.StopPerform(__instance);
				}
				return;
			}
			else if(Utils.GetIsBanned(CraftData.GetTechType(__instance.gameObject)))
			{
				// If we're a banned non-persistent creature, die immediately
				GameObject.Destroy(__instance.gameObject);
				return;
			}

			// If we're a non-banned non-persistent creature, do nothing
			return;
		}

		/*
		 * Here we skip the behavior update for PCs
		 * This means a PC will default to doing nothing when spawned.
		 * PC-derived classes promise to implement a postfix here
		 * TODO how do I enforce that promise?
		 */
		[HarmonyPrefix]
		[HarmonyPatch("UpdateBehaviour")]
		public static bool UpdateBehaviourPrefix(Creature __instance, List<CreatureAction> ___actions)
		{
			if (__instance.gameObject?.GetComponent<PersistentCreatureBehavior>() == null ||
				!__instance.gameObject.GetComponent<PersistentCreatureBehavior>().OverridesBehavior)
			{
				return true;
			}

			foreach (CreatureAction ca in ___actions)
			{
				ca.StopPerform(__instance);
			}
			return false;
		}

		[HarmonyPostfix]
		[HarmonyPatch("UpdateBehaviour")]
		public static void UpdateBehaviourPostfix(Creature __instance, List<CreatureAction> ___actions)
		{
			if (__instance.gameObject?.GetComponent<PersistentCreatureBehavior>() == null ||
				!__instance.gameObject.GetComponent<PersistentCreatureBehavior>().OverridesBehavior)
			{
				return;
			}

			foreach (CreatureAction ca in ___actions)
			{
				ca.StopPerform(__instance);
			}
			__instance.GetComponent<PersistentCreatureBehavior>().UpdateBehavior();
		}
	}
}
