using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;

namespace StealthModule
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[StealthModule] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[StealthModule] " + string.Format(format, args));
        }
    }

    public class StealthModulePatcher
    {
        internal static MyConfig Config { get; private set; }
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.stealthmodule.mod");
            harmony.PatchAll();
        }
    }

    public enum StealthQuality
    {
        None,
        Low,
        Medium,
        High
    }

    [Menu("StealthModule Options")]
    public class MyConfig : ConfigFile
    {
        [Choice("Stealth Quality")]
        public StealthQuality stealthQuality = StealthQuality.Medium;

    }
}
