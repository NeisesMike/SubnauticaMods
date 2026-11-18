using BepInEx;
using Nautilus.Handlers;

namespace SonarModule
{

    [BepInPlugin(pluginGUID, "SonarModule", "2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.sonarmodule.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();
            SonarModule module = new SonarModule();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            VehicleFramework.Admin.UpgradeCompat compat = new VehicleFramework.Admin.UpgradeCompat
            {
                skipCyclops = false,
                skipModVehicle = true,
                skipSeamoth = true,
                skipExosuit = true
            };
            CyclopsSonarModule Cyclopsmodule = new CyclopsSonarModule();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(Cyclopsmodule, compat);
            var harmony = new HarmonyLib.Harmony(pluginGUID);
            harmony.PatchAll();
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
                    Cyclopsmodule.TechTypes.forCyclops,
                    new string[] { "CyclopsMenu" }
                );
            }
        }
    }
}
