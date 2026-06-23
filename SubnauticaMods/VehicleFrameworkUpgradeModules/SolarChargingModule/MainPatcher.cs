using BepInEx;
using Nautilus.Handlers;

namespace SolarChargingModule
{
    [BepInPlugin(pluginGUID, "SolarChargingModule", "2.0")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.solarchargingmodule.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            MyConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            SolarChargingModule module = new SolarChargingModule();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            if (MyConfig.vanillaFabricator)
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
}
