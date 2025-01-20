using HarmonyLib;

namespace ThirdPerson
{
    [HarmonyPatch(typeof(Player))]
    public static class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Awake))]
        public static void StartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ThirdPersonCameraController>();
            return;
        }
    }
}
