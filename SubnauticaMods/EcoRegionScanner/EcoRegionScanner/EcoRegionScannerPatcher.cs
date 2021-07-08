using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;

namespace EcoRegionScanner
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[EcoRegionScanner] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[EcoRegionScanner] " + string.Format(format, args));
        }
    }
    public class EcoRegionScannerPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.ecoregionscanner.mod");
            harmony.PatchAll();
        }
    }

    [Menu("EcoRegion Scanner Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Toggle Scanner")]
        public bool isScannerActive = true;

        [Toggle("Fast Seamoth (Debug)")]
        public bool isFastSeamoth = true;

        [Keybind("Print Depth Map to File")]
        public KeyCode printMapKey = KeyCode.Backspace;
    }
}
