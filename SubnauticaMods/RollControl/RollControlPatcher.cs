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

        [Toggle("Enable Scuba Roll Unlimited (experimental)")]
        public bool ScubaRollUnlimited = true;

        [Slider("Seamoth Roll Speed", Min = 0f, Max = 1f, Step = 0.01f)]
        public double SeamothRollSpeed = 0.3f;

        [Slider("Scuba Roll Speed", Min = 0f, Max = 1f, Step = 0.01f)]
        public double ScubaRollSpeed = 0.3f;
    }

    [QModCore]
    public static class RollControlPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static bool isSeamothRollOn = false;
        public static bool isScubaRollOn = false;

        [QModPatch]
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
        }

        public static RollManager myRollMan = new RollManager();
    }

}
