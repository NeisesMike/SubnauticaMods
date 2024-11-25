using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(MoveTowardsTarget))]
    [HarmonyPatch(nameof(MoveTowardsTarget.UpdateCurrentTarget))]
    internal class MoveTowardsTargetUpdateCurrentTargetPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(MoveTowardsTarget __instance, ref IEcoTarget ___currentTarget)
        {
            // ensure this is HumanHunting Percy
            if (!ReaperManager.reaperDict.ContainsValue(__instance.gameObject) || PersistentReaperPatcher.PRConfig.reaperBehaviors != ReaperBehaviors.HumanHunter)
            {
                return true;
            }
            if (ReaperBehavior.IsValidTargetForPercy(__instance.gameObject))
            {
                // long-handed way of getting the right reaper behavior...
                // there's gotta be a better way...
                // maybe by ensuringComponent on ReaperBehavior...
                ReaperBehavior percyBehavior = null;
                foreach (KeyValuePair<ReaperBehavior, GameObject> entry in ReaperManager.reaperDict)
                {
                    if (entry.Value == __instance.gameObject)
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
}
