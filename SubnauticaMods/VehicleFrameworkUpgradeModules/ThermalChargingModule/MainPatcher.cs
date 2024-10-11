using BepInEx;

namespace ThermalChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.thermalchargingmodule.mod", "ThermalChargingModule", "1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new ThermalChargingModule());
        }
    }
}
