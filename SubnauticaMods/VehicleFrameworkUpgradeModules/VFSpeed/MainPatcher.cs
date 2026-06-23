using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using VehicleFramework.Assets;

namespace VFSpeed
{
    [BepInPlugin(pluginGUID, "VFSpeedModule", "2.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.speedmodule.mod";
        internal static SpeedOptions config { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();

            config = OptionsPanelHandler.RegisterModOptions<SpeedOptions>();
            var mk1 = new SpeedModuleMk1();
            var tt1 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk1);

            var mk2 = new SpeedModuleMk2();
            mk2.ExtendRecipe(tt1);
            var tt2 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk2);

            var mk3 = new SpeedModuleMk3();
            mk3.ExtendRecipe(tt2);
            var tt3 = VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(mk3);

            if (config.vanillaFabricator)
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "SpeedModulesMenu", Language.main.Get("Node_VFSpeedModules"), SpriteHelper.GetSprite("SpeedModuleIcon3.png"));
                CraftTreeHandler.AddTabNode(CraftTree.Type.CyclopsFabricator, "CyclopsMenu", Language.main.Get("Node_CyclopsMenu"), SpriteManager.Get(TechType.Cyclops));

                //mk1
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    mk1.TechTypes.forSeamoth,
                    new string[] { "SeamothModules" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.SeamothUpgrades,
                    mk1.TechTypes.forExosuit,
                    new string[] { "ExosuitModules" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.CyclopsFabricator,
                    mk1.TechTypes.forCyclops,
                    new string[] { "CyclopsMenu" }
                );

                //mk2
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk2.TechTypes.forSeamoth,
                    new string[] { "SpeedModulesMenu" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk2.TechTypes.forExosuit,
                    new string[] { "SpeedModulesMenu" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk2.TechTypes.forCyclops,
                    new string[] { "SpeedModulesMenu" }
                );

                //mk3
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk3.TechTypes.forSeamoth,
                    new string[] { "SpeedModulesMenu" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk3.TechTypes.forExosuit,
                    new string[] { "SpeedModulesMenu" }
                );
                CraftTreeHandler.AddCraftingNode(
                    CraftTree.Type.Workbench,
                    mk3.TechTypes.forCyclops,
                    new string[] { "SpeedModulesMenu" }
                );
            }
        }
    }
    [Menu("VF Speed Module Options")]
    public class SpeedOptions : ConfigFile
    {
        [Slider("Intensity", Tooltip = "Modify the per-upgrade bonus.", Min = 0.1f, Max = 5, Step = 0.1f)]
        public float intensity = 1f;

        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
}
