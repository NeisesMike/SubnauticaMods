using HarmonyLib;

namespace FreeLook
{
    [HarmonyPatch(typeof(Vehicle))]
    public static class VehiclePatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Vehicle.Update))]
        public static bool UpdatePrefix(Vehicle __instance)
        {
            foreach(var player in __instance.GetComponentsInChildren<Player>())
            {
                if (player.GetComponent<FreeLookManager>().isFreeLooking && player.GetVehicle() == __instance)
                {
                    return false;
                }
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Vehicle.OnDockedChanged))]
        public static bool VehicleOnDockedChangedHarmonyPostfix(Vehicle __instance)
        {
            foreach (var player in __instance.GetComponentsInChildren<Player>())
            {
                if (player.GetComponent<FreeLookManager>() != null)
                {
                    player.GetComponent<FreeLookManager>().HardReset();
                }
            }
            return true;
        }
    }
}
