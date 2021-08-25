using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;

namespace RollControl
{
    [Menu("RollControl Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("Roll Counter-Clockwise")]
        public KeyCode RollPortKey = KeyCode.Z;

        [Keybind("Roll Clockwise")]
        public KeyCode RollStarboardKey = KeyCode.C;

        [Keybind("Toggle Seamoth Roll")]
        public KeyCode SeamothRollToggleKey = KeyCode.RightAlt;

        [Keybind("Toggle Scuba Roll")]
        public KeyCode ScubaRollToggleKey = KeyCode.RightControl;

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
