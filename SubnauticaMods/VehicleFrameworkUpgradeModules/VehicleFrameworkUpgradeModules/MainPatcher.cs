using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace NanoWeaveBarrier
{
    [BepInPlugin("com.mikjaw.subnautica.nanoweavebarrier.mod", "NanoWeaveBarrier", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static NanoWeaveBarrierConfig NanoWeaveConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            NanoWeaveBarrier module = new NanoWeaveBarrier();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            NanoWeaveConfig = OptionsPanelHandler.RegisterModOptions<NanoWeaveBarrierConfig>();
            if (NanoWeaveConfig.vanillaFabricator)
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
                CraftTreeHandler.AddTabNode(CraftTree.Type.CyclopsFabricator, "CyclopsMenu", Language.main.Get("Node_CyclopsMenu"), SpriteManager.Get(TechType.Cyclops));
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.CyclopsFabricator,
                    module.TechTypes.forCyclops,
                    new string[] { "CyclopsMenu" }
                );
            }
        }
    }

    [Menu("Nano Weave Barrier Options")]
    public class NanoWeaveBarrierConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
}
