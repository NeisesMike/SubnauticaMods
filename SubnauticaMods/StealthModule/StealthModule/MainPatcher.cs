using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;
using VehicleFramework.Admin;
using HarmonyLib;

namespace StealthModule
{ 
    [BepInPlugin(pluginGUID, "StealthModule", "4.0.0")]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID, VehicleFramework.PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string pluginGUID = "com.mikjaw.subnautica.stealthmodule.mod";
        public static ManualLogSource logger { get; private set; }
        internal static MyConfig config { get; private set; }
        internal const string tabName = "StealthModules";
        internal const string tabDisplayName = "Stealth Modules";
        internal static UnityEngine.Sprite stealthIcon = VehicleFramework.Assets.SpriteHelper.GetSprite("assets/SeamothStealthModuleIcon.png");
        public void Start()
        {
            logger = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            RegisterUpgrades();
            var harmony = new Harmony(pluginGUID);
            harmony.PatchAll();
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

            var sum4 = new StealthUpgradeMk4();
            sum4.ExtendRecipe(tt3);
            UpgradeTechTypes tt4 = UpgradeRegistrar.RegisterUpgrade(sum4);

            var sum5 = new StealthUpgradeMk5();
            sum5.ExtendRecipe(tt4);
            UpgradeTechTypes tt5 = UpgradeRegistrar.RegisterUpgrade(sum5);
        }

        [Menu("Stealth Module Options")]
        public class MyConfig : ConfigFile
        {
            [Toggle("Enable Leviathan Distance Indicator")]
            public bool isDistanceIndicatorEnabled = true;

            [Toggle("Effect Logging", Tooltip = "Write in the BepInEx log whenever a stealth module causes a creature to not attack a target.")]
            public bool isEffectLogging = true;
        }
    }

}
