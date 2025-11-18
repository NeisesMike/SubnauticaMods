/*
using BepInEx.Bootstrap;
using VehicleFramework;
using VehicleFramework.Engines;
using HarmonyLib;

namespace FreeRead
{
    [HarmonyPatch(typeof(ModVehicle))]
    internal static class VehicleFrameworkHandler
    {
        internal static void HandleVF(ModVehicle vehicle)
        {
            if (HasVehicleFramework() && IsVehicleFrameworkVehicle(vehicle) && IsFreeReading(vehicle))
            {
                UseVehicleFrameworkEngine(vehicle);
            }
        }
        private static bool HasVehicleFramework()
        {
            return Chainloader.PluginInfos.ContainsKey(VehicleFramework.PluginInfo.PLUGIN_GUID);
        }
        private static bool IsFreeReading(Vehicle vehicle)
        {
            return vehicle.gameObject.EnsureComponent<FreeReadManager>().isFreeReading;
        }
        private static bool IsVehicleFrameworkVehicle(ModVehicle vehicle)
        {
            return vehicle is ModVehicle;
        }
        private static void UseVehicleFrameworkEngine(ModVehicle vehicle)
        {
            VFEngine engine = vehicle.GetComponent<ModVehicle>().VFEngine;
            if (engine == null) return;
            UnityEngine.Vector3 moveDirection = GameInput.GetMoveDirection();
            //engine.ApplyPlayerControls(moveDirection);
            //engine.DrainPower(moveDirection);
        }

        [HarmonyPatch(nameof(ModVehicle.FixedUpdate))]
        [HarmonyPostfix]
        public static void ModVehicleFixedUpdatePostfix(ModVehicle __instance)
        {
            ModVehicleEngine mve = __instance.VFEngine as ModVehicleEngine;
            if (__instance.GetIsUnderwater() || (mve != null && mve.CanMoveAboveWater)) // CanMove
            {
                if (__instance.CanPilot() && __instance.IsPlayerControlling()) // CanTakeInputs
                {
                    if (Player.main.GetPDA().isOpen) // When Not DoMovementInputs
                    {
                        VehicleFrameworkHandler.HandleVF(__instance);
                    }
                }
            }
        }
    }
}
*/
