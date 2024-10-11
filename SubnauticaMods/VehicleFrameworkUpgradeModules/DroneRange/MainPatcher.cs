using BepInEx;

namespace DroneRange
{

    [BepInPlugin("com.mikjaw.subnautica.dronerange.mod", "DroneRange", "1.2")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat
            {
                skipCyclops = true,
                skipModVehicle = false,
                skipSeamoth = true,
                skipExosuit = true
            };
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new DroneRangeUpgrade(), compat);
        }
    }
}
