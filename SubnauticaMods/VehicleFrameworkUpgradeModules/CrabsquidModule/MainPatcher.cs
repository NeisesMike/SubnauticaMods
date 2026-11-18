using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace CrabsquidModule
{
    [BepInPlugin("com.mikjaw.subnautica.crabsquidmodule.mod", "CrabsquidModule", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static CrabsquidProtectionConfig CrabquidConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            CrabsquidModule module = new CrabsquidModule();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            var harmony = new Harmony("com.mikjaw.subnautica.crabsquidmodule.mod");
            harmony.PatchAll();
            CrabquidConfig = OptionsPanelHandler.RegisterModOptions<CrabsquidProtectionConfig>();
            if (CrabquidConfig.vanillaFabricator)
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

    [Menu("Crabquid Protection Options")]
    public class CrabsquidProtectionConfig : ConfigFile
    {
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
}