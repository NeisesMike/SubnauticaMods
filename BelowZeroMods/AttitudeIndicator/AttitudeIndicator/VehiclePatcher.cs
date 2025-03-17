namespace AttitudeIndicator
{
    [HarmonyLib.HarmonyPatch(typeof(Vehicle))]
    [HarmonyLib.HarmonyPatch(nameof(Vehicle.Awake))]
    public class VehiclePatcher
    {
        [HarmonyLib.HarmonyPostfix]
        public static void VehicleAwakePostfix(Vehicle __instance)
        {
            if(__instance is SeaMoth || __instance is VehicleFramework.ModVehicle)
            {
                AssetGetter.SetupAttitudeIndicator(__instance.transform);
            }
        }
    }
}

