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

        [Toggle("Toggle Seamoth Roll"), OnChange(nameof(ChangeStabilization))]
        public bool SeamothRoll = true;
        public void ChangeStabilization(ToggleChangedEventArgs e)
        {
            foreach(SeaMoth sm in Transform.FindObjectsOfType<SeaMoth>())
            {
                sm.stabilizeRoll = e.Value;
            }
        }

        [Toggle("Toggle Scuba Roll")]
        public bool ScubaRoll = true;

        [Slider("Seamoth Roll Speed", Min = 0f, Max = 1f, Step = 0.01f, DefaultValue = 0.3f)]
        public double SeamothRollSpeed = 0.3f;

        [Slider("Scuba Roll Speed", Min = 0f, Max = 1f, Step = 0.01f, DefaultValue = 0.3f)]
        public double ScubaRollSpeed = 0.3f;

        [Slider("Scuba Look Sensitivity", Min = 0.30f, Max = 1.2f, Step = 0.01f, DefaultValue = 0.75f)]
        public float ScubaLookSensitivity = 0.75f;
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
