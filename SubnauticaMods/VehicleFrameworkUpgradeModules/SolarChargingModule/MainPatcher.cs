using BepInEx;

namespace SolarChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.solarchargingmodule.mod", "SolarChargingModule", "1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SolarChargingModule());
        }
    }
}
