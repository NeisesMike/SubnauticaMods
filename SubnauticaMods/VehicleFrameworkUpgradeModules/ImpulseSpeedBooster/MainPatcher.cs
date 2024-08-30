using BepInEx;

namespace ImpulseSpeedBooster
{

    [BepInPlugin("com.mikjaw.subnautica.impulsespeedbooster.mod", "ImpulseSpeedBooster", "1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.3.0")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static float invincibilityDuration = 3f;
        public static float maxCharge = 30f;
        public static float energyCost = 5f;
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new ImpulseSpeedBooster());
        }
    }
}
