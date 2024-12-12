using BepInEx;

namespace VFSpeed
{
    [BepInPlugin(pluginGUID, "VFSpeedModule", "1.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.speedmodule.mod";
        public void Start()
        {
            var tt1 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SpeedModuleMk1());

            var mk2 = new SpeedModuleMk2();
            mk2.ExtendRecipe(tt1);
            var tt2 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk2);

            var mk3 = new SpeedModuleMk3();
            mk3.ExtendRecipe(tt2);
            var tt3 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk3);
        }
    }
}
