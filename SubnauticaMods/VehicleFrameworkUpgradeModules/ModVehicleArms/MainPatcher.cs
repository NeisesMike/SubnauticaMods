using BepInEx;
using HarmonyLib;

namespace VFDrillArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfdrillarm.mod", "VFDrillArm", "1.4")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VFDrillArm());
            var harmony = new Harmony("com.mikjaw.subnautica.vfdrillarm.mod");
            harmony.PatchAll();
        }
    }
}
