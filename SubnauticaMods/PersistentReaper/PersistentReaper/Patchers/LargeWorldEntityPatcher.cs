using HarmonyLib;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(LargeWorldEntity))]
    [HarmonyPatch(nameof(LargeWorldEntity.Start))]
    public class LargeWorldEntityPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(LargeWorldEntity __instance)
        {
            if (ReaperManager.reaperDict.ContainsValue(__instance.gameObject))
            {
                return false;
            }
            return true;
        }
    }
}
