using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleFramework.Admin;
using BepInEx.Configuration;

namespace SelfRepairModuleUpgrade
{
    internal static class Configuration
    {
        internal const string amountOptionName = "Self Repair Amount";
        internal const string costOptionName = "Self Repair Cost";
        internal static void RegisterOptions()
        {
            const string amountOptionDescription = "How much does the upgrade repair?";
            ConfigRegistrar.RegisterForSeamoth<float>(amountOptionName, new ConfigDescription(amountOptionDescription, new AcceptableValueRange<float>(0, 10)), 5, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(amountOptionName, new ConfigDescription(amountOptionDescription, new AcceptableValueRange<float>(0, 10)), 5, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(amountOptionName, new ConfigDescription(amountOptionDescription, new AcceptableValueRange<float>(0, 10)), 5, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(amountOptionName, new ConfigDescription(amountOptionDescription, new AcceptableValueRange<float>(0, 10)), 5, null, MainPatcher.Instance.Config);

            const string costOptionDescription = "How much does the repair cost?";
            ConfigRegistrar.RegisterForSeamoth<float>(costOptionName, new ConfigDescription(costOptionDescription, new AcceptableValueRange<float>(0, 5)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForPrawn<float>(costOptionName, new ConfigDescription(costOptionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForCyclops<float>(costOptionName, new ConfigDescription(costOptionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
            ConfigRegistrar.RegisterForAllModVehicles<float>(costOptionName, new ConfigDescription(costOptionDescription, new AcceptableValueRange<float>(0, 1)), 1, null, MainPatcher.Instance.Config);
        }
    }
}
