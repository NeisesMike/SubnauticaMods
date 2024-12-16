using HarmonyLib;

namespace StealthModule
{
    [HarmonyPatch(typeof(Player))]
    class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void PlayerStartHarmonyPostfix(Player __instance)
        {
            __instance.gameObject.AddComponent<StealthModuleLogger>();
        }
    }
}
