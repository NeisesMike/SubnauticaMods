using BepInEx;

namespace ImpulseSpeedBooster
{

    [BepInPlugin("com.mikjaw.subnautica.impulsespeedbooster.mod", "ImpulseSpeedBooster", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static MainPatcher Instance { get; private set; }
        public static float invincibilityDuration = 3f;
        public static float maxCharge = 30f;
        public static float energyCost = 5f;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            if (Instance != this)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }
        public void Start()
        {
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new ImpulseSpeedBooster());
            SpeedConfig.RegisterOptions();
        }
    }
}
