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
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace EcoRegionScanner
{
    [BepInPlugin("com.mikjaw.subnautica.depthscanner.mod", "DepthScanner", "2.0.1")]
    [BepInDependency("com.snmodding.nautilus")]

    public class EcoRegionScannerPatcher : BaseUnityPlugin
    {
        public static ManualLogSource logger { get; set; }
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            logger = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.depthscanner.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Depth Scanner Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Toggle Scanner")]
        public bool isScannerActive = true;

        [Toggle("Fast Seamoth (Debug)")]
        public bool isFastSeamoth = false;

        [Keybind("Print Depth Map to File")]
        public KeyCode printMapKey = KeyCode.Backspace;
    }
}
