using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Utility;

namespace RollControl
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[RollControl] " + message);
        }
        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[RollControl] " + string.Format(format, args));
        }
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, 0);
            message.ShowMessage(msg, 5);
        }
    }

    [Menu("RollControl Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("Toggle Roll Key"), Tooltip("Roll is toggled individually for scuba mode and for each vehicle you have.")]
        public KeyCode ToggleRollKey = KeyCode.RightAlt;
        [Keybind("Roll Counter-Clockwise")]
        public KeyCode RollPortKey = KeyCode.Z;
        [Keybind("Roll Clockwise")]
        public KeyCode RollStarboardKey = KeyCode.C;
        [Slider("Submarine Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 30f)]
        public double SubmarineRollSpeed = 30f;
        [Slider("Scuba Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 75f)]
        public double ScubaRollSpeed = 75f;
        [Slider("Scuba Look Sensitivity", Min = 0f, Max = 200f, Step = 1f, DefaultValue = 30f)]
        public float ScubaLookSensitivity = 30f;
        [Toggle("Enable Vehicle Roll by Default")]
        public bool IsVehicleRollDefaultEnabled = true;
        [Toggle("Enable Scuba Roll by Default")]
        public bool IsScubaRollDefaultEnabled = true;
    }

    [QModCore]
    public static class RollControlPatcher
    {
        internal static MyConfig Config { get; private set; }

        [QModPatch]
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
        }
    }
}
