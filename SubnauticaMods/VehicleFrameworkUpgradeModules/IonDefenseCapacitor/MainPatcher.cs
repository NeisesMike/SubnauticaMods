using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace IonDefenseCapacitor
{
    [BepInPlugin("com.mikjaw.subnautica.iondefensecapacitor.mod", "IonDefenseCapacitor", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static IonDefenseCapacitorConfig IonDefenseConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            IonDefenseCapacitor module = new IonDefenseCapacitor();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            IonDefenseConfig = OptionsPanelHandler.RegisterModOptions<IonDefenseCapacitorConfig>();
            if (IonDefenseConfig.vanillaFabricator)
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

    [Menu("Ion Defense Capacitor Options")]
    public class IonDefenseCapacitorConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
}
