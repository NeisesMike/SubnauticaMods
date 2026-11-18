using BepInEx;
using Nautilus.Handlers;

namespace VoidDepth
{
    [BepInPlugin(pluginGUID, "VoidDepthUpgrade", "2.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.voiddepth.mod";
        internal static Config MyConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            MyConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            VoidDepth module = new VoidDepth();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
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
                    module.TechTypes.forCyclops,
                    new string[] { "CyclopsMenu" }
                );
            }
        }
    }
}
