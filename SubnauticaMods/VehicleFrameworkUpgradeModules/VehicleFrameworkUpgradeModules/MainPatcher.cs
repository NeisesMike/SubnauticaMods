using BepInEx;

namespace NanoWeaveBarrier
{
    [BepInPlugin("com.mikjaw.subnautica.nanoweavebarrier.mod", "NanoWeaveBarrier", "1.2")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new NanoWeaveBarrier());
        }
    }
}
