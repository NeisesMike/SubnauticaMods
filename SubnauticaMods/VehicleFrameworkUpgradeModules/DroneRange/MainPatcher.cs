using BepInEx;

namespace DroneRange
{

    [BepInPlugin("com.mikjaw.subnautica.dronerange.mod", "DroneRange", "1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.0")]
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
