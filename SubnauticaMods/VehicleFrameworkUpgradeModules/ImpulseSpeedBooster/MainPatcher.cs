using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace ImpulseSpeedBooster
{

    [BepInPlugin("com.mikjaw.subnautica.impulsespeedbooster.mod", "ImpulseSpeedBooster", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static ImpulseSpeedConfig ImpulseConfig { get; private set; }
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
            LanguageHandler.RegisterLocalizationFolder();
            ImpulseSpeedBooster module = new ImpulseSpeedBooster();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            SpeedConfig.RegisterOptions();
            ImpulseConfig = OptionsPanelHandler.RegisterModOptions<ImpulseSpeedConfig>();
            if (ImpulseConfig.vanillaFabricator)
            {
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    module.TechTypes.forSeamoth,
                    new string[] { "SeamothModules" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    module.TechTypes.forExosuit,
                    new string[] { "ExosuitModules" }
                );
            }
        }
    }

    [Menu("Impulse Speed Module Options")]
    public class ImpulseSpeedConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console. Restart required.")]
        public bool vanillaFabricator = false;
    }
}
