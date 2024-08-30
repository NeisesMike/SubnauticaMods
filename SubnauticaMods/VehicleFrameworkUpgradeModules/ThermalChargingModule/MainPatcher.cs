using BepInEx;

namespace ThermalChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.thermalchargingmodule.mod", "ThermalChargingModule", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new ThermalChargingModule());
        }
    }
}
