using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;

namespace StealthModule
{
    public static class VehicleFrameworkHandler
    {
        internal static ModVehicleStealthModule1 modVehicleStealthModule1;
        internal static ModVehicleStealthModule2 modVehicleStealthModule2;
        internal static ModVehicleStealthModule3 modVehicleStealthModule3;
        public static void PatchModVehicleModules(ref Harmony harmony)
        {
            EquipmentType ModVehicleUpgradeModuleType;
            if(!SMLHelper.V2.Handlers.EquipmentHandler.Main.TryGetModdedEquipmentType("ModVehicleModule", out ModVehicleUpgradeModuleType))
            {
                Logger.Log("ERROR: Failed to recover ModVehicle Upgrade Equipment Type");
            }

            modVehicleStealthModule1 = new ModVehicleStealthModule1(ModVehicleUpgradeModuleType);
            modVehicleStealthModule2 = new ModVehicleStealthModule2(ModVehicleUpgradeModuleType);
            modVehicleStealthModule3 = new ModVehicleStealthModule3(ModVehicleUpgradeModuleType);
            modVehicleStealthModule1.Patch();
            modVehicleStealthModule2.Patch();
            modVehicleStealthModule3.Patch();

            var ModVehicleType = Type.GetType("VehicleFramework.ModVehicle, VehicleFramework", false, false);

            var AwakeMethod = AccessTools.Method(ModVehicleType, "Awake");
            var AwakePostfix = new HarmonyMethod(AccessTools.Method(typeof(MVAwakePatcher), "Postfix"));
            harmony.Patch(AwakeMethod, AwakePostfix);

            var OnUpModChangeMethod = AccessTools.Method(ModVehicleType, "OnUpgradeModuleChange");
            var OnUpMCPostfix = new HarmonyMethod(AccessTools.Method(typeof(MVOnUpgradeModuleChangePatcher), "Postfix"));
            harmony.Patch(OnUpModChangeMethod, OnUpMCPostfix);
        }
    }
}
