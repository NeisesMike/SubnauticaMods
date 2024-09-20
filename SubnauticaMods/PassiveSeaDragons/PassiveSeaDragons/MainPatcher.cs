using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using HarmonyLib;

namespace PassiveSeaDragons
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PassiveSeaDragons] " + message);
        }
    }

    [BepInPlugin("com.mikjaw.subnautica.passiveseadragons.mod", "PassiveSeaDragons", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.passiveseadragons.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Passive Sea Dragon Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Sea Dragons Ignore Player")]
        public bool ignorePlayer = true;

        [Toggle("Sea Dragons Ignore Cyclops")]
        public bool ignoreCyclops = true;

        [Toggle("Sea Dragons Ignore Vehicles", Tooltip = "Sea Dragons will not grab or damage exosuits")]
        public bool ignoreVehicles = true;

        [Toggle("Sea Dragon Disable Damage", Tooltip = "Bite damage and swat damage")]
        public bool isNoDamage = true;
    }

}








