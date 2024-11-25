using BepInEx;

namespace SonarModule
{

    [BepInPlugin(pluginGUID, "SonarModule", "1.3")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.sonarmodule.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SonarModule());
            var harmony = new HarmonyLib.Harmony(pluginGUID);
            harmony.PatchAll();
        }
    }
}
