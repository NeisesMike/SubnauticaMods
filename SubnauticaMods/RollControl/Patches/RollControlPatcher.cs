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
    public static class Logger
    {
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, -75);
            message.ShowMessage(msg, 5);
        }
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo("[RollControl] " + message);
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
        [Slider("Scuba Mouse Sensitivity", Min = 0f, Max = 200f, Step = 1f, DefaultValue = 30f, Tooltip = "How fast the camera rotates as you move the mouse")]
        public float ScubaMouseSensitivity = 30f;
        [Slider("Scuba Controller Sensitivity", Min = 0f, Max = 200f, Step = 1f, DefaultValue = 30f, Tooltip = "How fast the camera rotates as you tilt the analog stick")]
        public float ScubaPadSensitivity = 15f;
        [Toggle("Enable Vehicle Roll by Default")]
        public bool IsVehicleRollDefaultEnabled = false;
        [Toggle("Enable Scuba Roll by Default")]
        public bool IsScubaRollDefaultEnabled = false;
    }

    [BepInPlugin("com.mikjaw.subnautica.rollcontrol.mod", "RollControl", "5.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class RollControlPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        public static bool ThereIsVehicleFramework = false;
        public void Start()
        {
            RollControl.Logger.MyLog = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
            var type = Type.GetType("VehicleFramework.MainPatcher, VehicleFramework", false, false);
            if (type != null)
            {
                ThereIsVehicleFramework = true;
            }
        }
    }
}
