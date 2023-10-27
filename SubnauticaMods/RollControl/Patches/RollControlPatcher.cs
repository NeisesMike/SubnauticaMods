using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using Nautilus.Options;
using UnityEngine;
using HarmonyLib;
using Nautilus.Utility;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace RollControl
{
    public static class SubLog
    {
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, -75);
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

    [BepInPlugin("com.mikjaw.subnautica.rollcontrol.mod", "RollControl", "5.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class RollControlPatcher : BaseUnityPlugin
    {
        public static ManualLogSource logger { get; set; }
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            logger = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
        }
    }
}
