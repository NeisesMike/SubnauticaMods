namespace ThePlanets
{
    [HarmonyLib.HarmonyPatch(typeof(Player))]
    public static class PlayerPatcher
    {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(nameof(Player.Awake))]
        public static void AwakePostfix()
        {
            new UnityEngine.GameObject().AddComponent<ConsoleCommands>();
        }
    }
}
