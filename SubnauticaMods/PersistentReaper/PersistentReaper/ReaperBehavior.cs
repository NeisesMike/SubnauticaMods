using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PersistentReaper
{
    public class ReaperBehavior
    {
        public Int3 currentRegion = Int3.zero;
        public bool isLockedOntoPlayer = false;

        public static bool isValidTargetForPercy(GameObject Percy)
        {
            // Percy only hunts humans
            GameObject target = Player.main.gameObject;
            Vehicle thisPossibleVehicle = Player.main.currentMountedVehicle;

            //=============================================
            // Determine whether Percy can see or hear us.
            // His eyes are sharper than normal,
            // but his hearing is superb.
            //=============================================

            // First check if Percy can hear us.
            // Sound travels quite far underwater,
            // and doesn't require any specific facing.
            // TODO: configure this value
            // 90 is just a guess, based off of Stealth Module intel
            if (90 < Vector3.Distance(target.transform.position, Percy.transform.position))
            {
                // Too far away even to hear us.
                return false;
            }

            // determine what kind of sound the player is making
            // seamoth noise
            // cyclops noise
            // prawn noise
            // TODO: tool noise
            // TODO: for now, only vehicles make sufficient noise for Percy to hear
            // TODO: configure these constants
            bool isWithinEarshot = !Physics.Linecast(Percy.transform.position, target.transform.position, Voxeland.GetTerrainLayerMask());
            bool isPlayerLoud;

            if (thisPossibleVehicle)
            {
                if (Player.main.inSeamoth)
                {
                    isPlayerLoud = 6 < Player.main.GetComponent<Rigidbody>().velocity.magnitude;
                }
                else if (Player.main.inExosuit)
                {
                    isPlayerLoud = 2 < Player.main.GetComponent<Rigidbody>().velocity.magnitude;
                }
                else if (Player.main.currentSub)
                {
                    isPlayerLoud = 1 < Player.main.GetComponent<Rigidbody>().velocity.magnitude && !Player.main.currentSub.silentRunning;
                }
                else
                {
                    isPlayerLoud = false;
                }
            }
            else
            {
                isPlayerLoud = false;
            }

            if (isWithinEarshot && isPlayerLoud)
            {
                // Percy has heard us.
                return true;
            }

            // If Percy couldn't hear us,
            // there's still a chance he could see us,
            // and Percy can see extra far.
            // TODO: configure this value
            // 75 is just a guess, based off of Stealth Module intel
            if (75 < Vector3.Distance(target.transform.position, Percy.transform.position))
            {
                // Too far away to see us.
                return false;
            }
            if (Percy.GetComponent<Creature>().GetCanSeeObject(target))
            {
                // Percy has seen us.
                return true;
            }

            // If Persistent Percy failed to perceive us,
            // the target is invalid.
            return false;
        }
    }

    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    [HarmonyPatch("GetAggressionTarget")]
    internal class AggressiveWhenSeeTargetGetAggressionTargetPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(AggressiveWhenSeeTarget __instance, ref GameObject __result)
        {
            // ensure this is HumanHunting Percy
            if (!ReaperManager.reaperDict.ContainsValue(__instance.gameObject) || MainPatcher.PRConfig.reaperBehaviors != ReaperBehaviors.HumanHunter)
            {
                return true;
            }
            if (ReaperBehavior.isValidTargetForPercy(__instance.gameObject))
            {
                // Percy has only one target.
                __result = Player.main.gameObject;
            }
            else
            {
                __result = null;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MoveTowardsTarget))]
    [HarmonyPatch("UpdateCurrentTarget")]
    internal class MoveTowardsTargetUpdateCurrentTargetPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(MoveTowardsTarget __instance, ref IEcoTarget ___currentTarget)
        {
            // ensure this is HumanHunting Percy
            if (!ReaperManager.reaperDict.ContainsValue(__instance.gameObject) || MainPatcher.PRConfig.reaperBehaviors != ReaperBehaviors.HumanHunter)
            {
                return true;
            }
            if(ReaperBehavior.isValidTargetForPercy(__instance.gameObject))
            {
                // long-handed way of getting the right reaper behavior...
                // there's gotta be a better way...
                // maybe by ensuringComponent on ReaperBehavior...
                ReaperBehavior percyBehavior = null;
                foreach (KeyValuePair<ReaperBehavior, GameObject> entry in ReaperManager.reaperDict)
                {
                    if(entry.Value == __instance.gameObject)
                    {
                        percyBehavior = entry.Key;
                        break;
                    }
                }

                if (percyBehavior.isLockedOntoPlayer)
                {
                    ___currentTarget = Player.main.gameObject.GetComponent<IEcoTarget>();
                }
            }
            else
            {
                ___currentTarget = null;
            }
            return false;
        }
    }

    /*
	[HarmonyPatch(typeof(AttackCyclops))]
	[HarmonyPatch("StartPerform")]
	class AttackCyclopsStartPerformPatcher
    {
		[HarmonyPrefix]
		public static bool Prefix(AttackCyclops __instance, Creature creature, ref GameObject ___currentTarget)
		{
			// check whether we're Percy and locked onto the player
			if (!(ReaperManager.reaperDict.ContainsKey(__instance.gameObject) && ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer))
			{
				return true;
			}

			___currentTarget = Player.main.gameObject;
			return true;
		}
    }

    [HarmonyPatch(typeof(AttackCyclops))]
    [HarmonyPatch("SetCurrentTarget")]
    class AttackCyclopsSetCurrentTargetPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(AttackCyclops __instance, ref GameObject target, bool isDecoy)
        {
            // check whether we're Percy and locked onto the player
            if (!(ReaperManager.reaperDict.ContainsKey(__instance.gameObject) && ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer))
            {
                return true;
            }

            target = Player.main.gameObject;
            return true;
        }
    }

    [HarmonyPatch(typeof(AttackLastTarget))]
	[HarmonyPatch("StartPerform")]
	class AttackLastTargetStartPerformPatcher
    {
		[HarmonyPrefix]
		public static bool Prefix(AttackLastTarget __instance, Creature creature, ref GameObject ___currentTarget)
		{
			// check whether we're Percy and locked onto the player
			if (!(ReaperManager.reaperDict.ContainsKey(__instance.gameObject) && ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer))
			{
				return true;
			}

			___currentTarget = Player.main.gameObject;

            return true;
		}
	}
    */
}
