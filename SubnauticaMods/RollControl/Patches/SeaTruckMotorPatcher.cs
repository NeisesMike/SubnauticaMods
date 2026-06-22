using HarmonyLib;

namespace RollControl
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("StabilizeRoll")]
    class SeaTruckMotorStabilizeRollPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance)
        {
            VehicleRollController manager = __instance.gameObject.EnsureComponent<VehicleRollController>();
            if (manager.isRollEnabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
