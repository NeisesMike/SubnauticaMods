using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace FlightModule
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "com.mikjaw.subnautica.flightmodule.mod";
        private const string PLUGIN_NAME = "FlightModule";
        private const string PLUGIN_VERSION = "2.0";
        internal static FlightModuleConfig FlightConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat
            {
                skipCyclops = false,
                skipExosuit = true,
                skipModVehicle = false,
                skipSeamoth = false
            };
            FlightModule module = new FlightModule();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module, compat);
            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll();

            FlightConfig = OptionsPanelHandler.RegisterModOptions<FlightModuleConfig>();
            if (FlightConfig.vanillaFabricator)
            {
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    module.TechTypes.forSeamoth,
                    new string[] { "SeamothModules" }
                );
                CraftTreeHandler.AddTabNode(CraftTree.Type.CyclopsFabricator, "CyclopsMenu", Language.main.Get("Node_CyclopsMenu"), SpriteManager.Get(TechType.Cyclops));
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.CyclopsFabricator,
                    module.TechTypes.forCyclops,
                    new string[] { "CyclopsMenu" }
                );
            }
        }
    }

    [Menu("Flight Module Options")]
    public class FlightModuleConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
}