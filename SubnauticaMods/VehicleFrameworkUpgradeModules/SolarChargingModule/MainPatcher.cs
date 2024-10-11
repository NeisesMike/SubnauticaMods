using BepInEx;

namespace SolarChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.solarchargingmodule.mod", "SolarChargingModule", "1.2")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SolarChargingModule());
        }
    }
}
