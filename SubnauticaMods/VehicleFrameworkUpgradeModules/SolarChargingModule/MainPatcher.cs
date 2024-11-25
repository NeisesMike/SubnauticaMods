using BepInEx;
using Nautilus.Handlers;

namespace SolarChargingModule
{
    [BepInPlugin(pluginGUID, "SolarChargingModule", "1.3")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.solarchargingmodule.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            MyConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SolarChargingModule());
        }
    }
}
