using BepInEx;
using HarmonyLib;

namespace VFDrillArm
{
    [BepInPlugin("com.mikjaw.subnautica.vfdrillarm.mod", "VFDrillArm", "2.0")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat
            {
                skipCyclops = true,
                skipModVehicle = false,
                skipSeamoth = true,
                skipExosuit = !MyConfig.isPrawnArm
            };
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new VFDrillArm(), compat);
            var harmony = new Harmony("com.mikjaw.subnautica.vfdrillarm.mod");
            harmony.PatchAll();
        }
    }
}
