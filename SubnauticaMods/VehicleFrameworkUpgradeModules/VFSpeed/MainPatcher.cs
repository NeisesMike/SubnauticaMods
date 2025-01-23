using BepInEx;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;

namespace VFSpeed
{
    [BepInPlugin(pluginGUID, "VFSpeedModule", "1.2")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.speedmodule.mod";
        internal static SpeedOptions config { get; private set; }
        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<SpeedOptions>();
            var tt1 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SpeedModuleMk1());

            var mk2 = new SpeedModuleMk2();
            mk2.ExtendRecipe(tt1);
            var tt2 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk2);

            var mk3 = new SpeedModuleMk3();
            mk3.ExtendRecipe(tt2);
            var tt3 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk3);
        }
    }
    [Menu("VF Speed Module Options")]
    public class SpeedOptions : ConfigFile
    {
        [Slider("Intensity", Tooltip = "Modify the per-upgrade bonus.", Min = 0.1f, Max = 5, Step = 0.1f)]
        public float intensity = 1f;
    }
}
