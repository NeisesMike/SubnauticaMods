using BepInEx;

namespace ImpulseSpeedBooster
{

    [BepInPlugin("com.mikjaw.subnautica.impulsespeedbooster.mod", "ImpulseSpeedBooster", "1.2")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
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
