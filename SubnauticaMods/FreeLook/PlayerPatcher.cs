using HarmonyLib;

namespace FreeLook
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        public static bool Prefix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<FreeLookManager>();
            return true;
        }
    }
}