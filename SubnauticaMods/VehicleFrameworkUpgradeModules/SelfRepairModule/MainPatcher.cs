using Nautilus.Handlers;

namespace SelfRepairModule
{
    [BepInEx.BepInPlugin(pluginGUID, "SelfRepairModule", "1.1.0")]
    [BepInEx.BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    [BepInEx.BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, "2.0.5")]
    public partial class MainPatcher : BepInEx.BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.selfrepairmodule.mod";
        public static MainPatcher Instance { get; private set; }
        internal static NautilusConfig MyConfig { get; private set; }
        public void Start()
        {
            LanguageHandler.RegisterLocalizationFolder();
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<NautilusConfig>();
            MyConfig = OptionsPanelHandler.RegisterModOptions<NautilusConfig>();
            SelfRepairModuleUpgrade module = new SelfRepairModuleUpgrade();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(module);
            Configuration.RegisterOptions();
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
    }
}
