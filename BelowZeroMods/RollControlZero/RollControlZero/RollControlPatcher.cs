using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Utility;
using BepInEx;
using BepInEx.Logging;

namespace RollControlZero
{
    public static class Logger
    {
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo(message);
        }
        public static void Output(string msg, int x = 500, int y = 0)
        {
            BasicText message = new BasicText(x, y);
            message.ShowMessage(msg, 4);
        }
    }

    [Menu("RollControl Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Roll Enabled")]
        public bool isSeatruckRollOn = true;
        [Keybind("Roll Toggle")]
        public KeyCode rollToggleKey = KeyCode.AltGr;
        [Keybind("Roll To Port")]
        public KeyCode rollToPortKey = KeyCode.Z;
        [Keybind("Roll To Starboard")]
        public KeyCode rollToStarboardKey = KeyCode.C;
        [Slider("Roll Speed Slider", 1, 30, DefaultValue = 15)]
        public float seatruckRollSpeed = 15;
        [Toggle("Roll HUD Element")]
        public bool isHUD = true;
        [Choice("Roll HUD Placement")]
        public TextAnchor HUDAnchor = TextAnchor.LowerRight;
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    public class RollControlPatcher : BaseUnityPlugin
    {
        internal static MyConfig RCConfig { get; private set; }
        public void Start()
        {
            RCConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}

