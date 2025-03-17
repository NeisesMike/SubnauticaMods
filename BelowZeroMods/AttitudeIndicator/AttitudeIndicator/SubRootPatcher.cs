namespace AttitudeIndicator
{
    [HarmonyLib.HarmonyPatch(typeof(SubRoot))]
    [HarmonyLib.HarmonyPatch(nameof(SubRoot.Awake))]
    public class SubRootPatcher
    {
        [HarmonyLib.HarmonyPostfix]
        public static void SubRootAwakeHarmonyPostfix(SubRoot __instance)
        {
            if(__instance.isCyclops)
            {
                AssetGetter.SetupAttitudeIndicator(__instance.transform);
            }
        }
    }
}

