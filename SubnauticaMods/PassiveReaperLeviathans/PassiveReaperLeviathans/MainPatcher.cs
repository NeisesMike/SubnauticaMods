using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using HarmonyLib;

namespace PassiveReaperLeviathans
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PassiveReaperLeviathans] " + message);
        }
    }

    [BepInPlugin("com.mikjaw.subnautica.passivereaperleviathans.mod", "PassiveReaperLeviathans", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.passivereaperleviathans.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Passive Reaper Leviathan Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Reapers Ignore Player")]
        public bool ignorePlayer = true;

        [Toggle("Reapers Ignore Cyclops")]
        public bool ignoreCyclops = true;

        [Toggle("Reapers Ignore Vehicles", Tooltip = "Seamoth, Prawn, etc")]
        public bool ignoreVehicles = true;

        [Toggle("Reaper Disable Bite Damage")]
        public bool isNoBiteDamage = true;

    }


}








