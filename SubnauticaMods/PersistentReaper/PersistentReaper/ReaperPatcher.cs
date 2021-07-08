using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(ReaperLeviathan))]
    [HarmonyPatch("Update")]
    public class ReaperUpdatePatcher
    {
        // updateInterval is measured in seconds
        private static float updateInterval = 0.025f;
        private static float lastUpdateTime = Time.time;

        [HarmonyPostfix]
        public static void Postfix(ReaperLeviathan __instance)
        {
            // check whether we're Percy
            if (!ReaperManager.reaperDict.ContainsKey(__instance.gameObject))
            {
                return;
            }
            if (PersistentReaperPatcher.Config.reaperBehaviors != ReaperBehaviors.Normal)
            {
                // these go to a max value of 1
                // these normally grow at a rate of deltaTime...
                // we'll triple the rate at which they become hungry and aggressive
                __instance.Hunger.UpdateTrait(Time.deltaTime * 2);
                __instance.Aggression.UpdateTrait(Time.deltaTime * 2);
            }

            // HumanHunter.Update
            if (PersistentReaperPatcher.Config.reaperBehaviors == ReaperBehaviors.HumanHunter && lastUpdateTime + updateInterval < Time.time)
            {
                // if we can see or hear the player, lock on
                if (ReaperBehavior.isValidTargetForPercy( __instance.gameObject))
                {
                    ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer = true;
                }

                Vector3 nextScentLoc = ReaperManager.tryMoveToScent(__instance.transform.position);
                if (nextScentLoc == Vector3.zero)
                {
                    return;
                }

                // if we don't really know where the player is,
                // make sure we're following their scent
                if (!ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer
                    && __instance.GetComponentInParent<SwimBehaviour>().splineFollowing.targetPosition != nextScentLoc)
                {
                    __instance.GetComponentInParent<SwimBehaviour>().SwimTo(nextScentLoc, 5f);
                }

                // if we're locked on, go for the attack
                if (ReaperManager.reaperDict[__instance.gameObject].isLockedOntoPlayer)
                {
                    Vector3 targetDirection = -MainCamera.camera.transform.forward;
                    __instance.GetComponentInParent<SwimBehaviour>().Attack(Player.main.transform.position, targetDirection, 10f);
                }

                lastUpdateTime = Time.time;
            }
        }
    }
}
