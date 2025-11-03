using BepInEx;

namespace FlightModule
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "com.mikjaw.subnautica.flightmodule.mod";
        private const string PLUGIN_NAME = "FlightModule";
        private const string PLUGIN_VERSION = "2.0";
        public void Start()
        {
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat
            {
                skipCyclops = false,
                skipExosuit = true,
                skipModVehicle = false,
                skipSeamoth = false
            };
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new FlightModule(), compat);

            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll();
        }
    }
}