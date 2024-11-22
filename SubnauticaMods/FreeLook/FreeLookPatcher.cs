﻿using System;
using UnityEngine;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using BepInEx;

namespace FreeLook
{
    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Enable Hinting")]
        public bool isHintingEnabled = true;

        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;

        [Toggle("Toggle FreeLook", Tooltip = "Enable this to have FreeLook toggle on and off, instead of requiring you to hold the button to FreeLook.")]
        public bool isToggle = false;

        [Slider("Trigger Deadzone %", Tooltip = "Add deadzone to the freelook input on analog-based triggers. Higher number means more deadzone.", Min = 0, Max = 100, Step = 1)]
        public int deadzone = 20;
    }

    [BepInPlugin("com.mikjaw.subnautica.freelook.mod", "FreeLook", "2.2.0")]
    [BepInDependency("com.snmodding.nautilus")]
    public class FreeLookPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.freelook.mod");
            harmony.PatchAll();

            // here we coerce tweaks and fixes into compatibility
            // in other words, we neuter one of its patches.
            var type2 = Type.GetType("Tweaks_Fixes.SeaMoth_patch, Tweaks and Fixes", false, false);
            if (type2 != null)
            {
                var TweaksFixesSeamothUpdatePrefix = AccessTools.Method(type2, "UpdatePrefix");
                harmony.Patch(TweaksFixesSeamothUpdatePrefix, prefix: new HarmonyMethod(typeof(FreeLookPatcher), nameof(TweaksFixesSeamothUpdatePrefixPrefix)));
            }
        }

        public static bool TweaksFixesSeamothUpdatePrefixPrefix(SeaMoth __instance, ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
