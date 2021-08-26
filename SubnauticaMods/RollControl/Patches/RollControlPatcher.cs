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
            UnityEngine.Debug.Log("[DebugScene] " + message);
        }
        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[DebugScene] " + string.Format(format, args));
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
        [Keybind("Roll Counter-Clockwise")]
        public KeyCode RollPortKey = KeyCode.Z;
        [Keybind("Roll Clockwise")]
        public KeyCode RollStarboardKey = KeyCode.C;

        [Toggle("Toggle Submarine Roll"), OnChange(nameof(ChangeStabilization))]
        public bool SubRoll = true;
        public void ChangeStabilization(ToggleChangedEventArgs e)
        {
            foreach(Vehicle veh in Transform.FindObjectsOfType<Vehicle>())
            {
                veh.stabilizeRoll = e.Value;
            }
        }
        [Slider("Submarine Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 30f)]
        public double SubmarineRollSpeed = 30f;

        [Toggle("Toggle Scuba Roll")]
        public bool ScubaRoll = true;
        [Slider("Scuba Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 75f)]
        public double ScubaRollSpeed = 75f;
        [Slider("Scuba Look Sensitivity", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 30f)]
        public float ScubaLookSensitivity = 30f;
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
