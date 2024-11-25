using BepInEx;

namespace VoidDepth
{
    [BepInPlugin(pluginGUID, "VoidDepthUpgrade", "1.1")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.voiddepth.mod";
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VoidDepth());
            var harmony = new HarmonyLib.Harmony(pluginGUID);
            harmony.PatchAll();
        }
    }
}
