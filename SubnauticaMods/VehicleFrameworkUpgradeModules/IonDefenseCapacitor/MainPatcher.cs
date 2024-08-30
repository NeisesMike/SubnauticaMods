using BepInEx;

namespace IonDefenseCapacitor
{
    [BepInPlugin("com.mikjaw.subnautica.iondefensecapacitor.mod", "IonDefenseCapacitor", "1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new IonDefenseCapacitor());
        }
    }
}
