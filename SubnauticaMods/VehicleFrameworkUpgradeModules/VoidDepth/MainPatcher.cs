using BepInEx;

namespace VoidDepth
{
    [BepInPlugin("com.mikjaw.subnautica.voiddepth.mod", "VoidDepthUpgrade", "1.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VoidDepth());
        }
    }
}
