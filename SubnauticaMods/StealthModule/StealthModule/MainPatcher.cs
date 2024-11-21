using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;
using VehicleFramework.Admin;

namespace StealthModule
{ 
    [BepInPlugin("com.mikjaw.subnautica.stealthmodule.mod", "StealthModule", "3.0.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static ManualLogSource logger { get; private set; }
        internal static MyConfig config { get; private set; }
        internal const string tabName = "StealthModules";
        internal const string tabDisplayName = "Stealth Modules";
        internal static Atlas.Sprite stealthIcon = VehicleFramework.Assets.SpriteHelper.GetSprite("assets/SeamothStealthModuleIcon.png");
        public void Start()
        {
            logger = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            RegisterUpgrades();
        }

        public void RegisterUpgrades()
        {
            UpgradeTechTypes tt1 = UpgradeRegistrar.RegisterUpgrade(new StealthUpgradeMk1());

            var sum2 = new StealthUpgradeMk2();
            sum2.ExtendRecipe(tt1);
            UpgradeTechTypes tt2 = UpgradeRegistrar.RegisterUpgrade(sum2);

            var sum3 = new StealthUpgradeMk3();
            sum3.ExtendRecipe(tt2);
            UpgradeTechTypes tt3 = UpgradeRegistrar.RegisterUpgrade(sum3);
        }

        [Menu("Stealth Module Options")]
        public class MyConfig : ConfigFile
        {
            [Toggle("Enable Leviathan Distance Indicator")]
            public bool isDistanceIndicatorEnabled = true;
        }
    }

}
