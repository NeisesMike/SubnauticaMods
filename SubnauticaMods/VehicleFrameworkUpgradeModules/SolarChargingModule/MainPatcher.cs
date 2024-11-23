using BepInEx;
using Nautilus.Handlers;

namespace SolarChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.solarchargingmodule.mod", "SolarChargingModule", "1.3")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            MyConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SolarChargingModule());
        }
    }
}
