using BepInEx;

namespace VoidDepth
{
    [BepInPlugin(pluginGUID, "VoidDepthUpgrade", "1.4")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.voiddepth.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VoidDepth());
            var harmony = new HarmonyLib.Harmony(pluginGUID);
            harmony.PatchAll();
        }
    }
}
