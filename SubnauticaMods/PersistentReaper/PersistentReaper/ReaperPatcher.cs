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
            if (!ReaperManager.reaperDict.ContainsValue(__instance.gameObject))
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
                ReaperBehavior percyBehavior = null;
                // we're guaranteed to find a value here, due to the earlier ContainsValue call
                foreach (KeyValuePair<ReaperBehavior, GameObject> entry in ReaperManager.reaperDict)
                {
                    if (entry.Value == __instance.gameObject)
                    {
                        percyBehavior = entry.Key;
                        break;
                    }
                }

                // if we can see or hear the player, lock on
                if (ReaperBehavior.isValidTargetForPercy(__instance.gameObject))
                {
                    percyBehavior.isLockedOntoPlayer = true;
                }

                Vector3 nextScentLoc = ReaperManager.tryMoveToScent(__instance.transform.position);
                if (nextScentLoc == Vector3.zero)
                {
                    return;
                }

                // if we don't know where the player is,
                // follow their scent
                if (!percyBehavior.isLockedOntoPlayer
                    && __instance.GetComponentInParent<SwimBehaviour>().splineFollowing.targetPosition != nextScentLoc)
                {
                    // TODO: I'm not sure this is working as intended...
                    // In practice, I think percy drops the trail
                    // We must somehow make percy more one-minded
                    __instance.GetComponentInParent<SwimBehaviour>().SwimTo(nextScentLoc, 5f);
                }

                // if we're locked on, go for the attack
                if (percyBehavior.isLockedOntoPlayer)
                {
                    Vector3 targetDirection = -MainCamera.camera.transform.forward;
                    __instance.GetComponentInParent<SwimBehaviour>().Attack(Player.main.transform.position, targetDirection, 10f);
                }

                // if there's no player and no scent trail,
                // just do whatever

                lastUpdateTime = Time.time;
            }
        }
    }
}
