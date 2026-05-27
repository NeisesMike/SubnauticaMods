using BepInEx;
using Nautilus.Handlers;

namespace ThermalChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.thermalchargingmodule.mod", "ThermalChargingModule", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new ThermalChargingModule());
        }
    }
}
