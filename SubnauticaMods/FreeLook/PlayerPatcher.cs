using HarmonyLib;

namespace FreeLook
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void PlayerStartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<FreeLookManager>();
        }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.ExitLockedMode))]
        public static void PlayerExitLockedModePostfix(Player __instance)
        {
            if (__instance.GetMode() == Player.Mode.Normal)
            {
                FreeLookManager flm = __instance.gameObject.EnsureComponent<FreeLookManager>();
                flm.isToggled = false;
                flm.isFreeLooking = false;
            }
        }
    }
}