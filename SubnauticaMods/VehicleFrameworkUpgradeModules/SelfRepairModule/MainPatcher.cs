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
            MyConfig = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<NautilusConfig>();
            VehicleFramework.Admin.UpgradeRegistrar.RegisterUpgrade(new SelfRepairModuleUpgrade());
            Configuration.RegisterOptions();
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
