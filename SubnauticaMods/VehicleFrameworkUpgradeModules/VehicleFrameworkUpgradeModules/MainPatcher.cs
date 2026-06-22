using BepInEx;
using Nautilus.Handlers;

namespace NanoWeaveBarrier
{
    [BepInPlugin("com.mikjaw.subnautica.nanoweavebarrier.mod", "NanoWeaveBarrier", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new NanoWeaveBarrier());
        }
    }
}
