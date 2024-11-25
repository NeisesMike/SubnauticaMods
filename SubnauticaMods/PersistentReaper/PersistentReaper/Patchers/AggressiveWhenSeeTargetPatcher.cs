using HarmonyLib;
using UnityEngine;

namespace PersistentReaper
{

    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    [HarmonyPatch(nameof(AggressiveWhenSeeTarget.GetAggressionTarget))]
    internal class AggressiveWhenSeeTargetGetAggressionTargetPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(AggressiveWhenSeeTarget __instance, ref GameObject __result)
        {
            // ensure this is HumanHunting Percy
            if (!ReaperManager.reaperDict.ContainsValue(__instance.gameObject) || PersistentReaperPatcher.PRConfig.reaperBehaviors != ReaperBehaviors.HumanHunter)
            {
                return true;
            }
            if (ReaperBehavior.IsValidTargetForPercy(__instance.gameObject))
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
}
