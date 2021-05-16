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

namespace FreeRead
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeRead] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[FreeRead] " + string.Format(format, args));
        }
    }
    public class FreeReadPatcher
    {
        public static bool isCruising = false;
        internal static MyConfig Config { get; private set; }

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.garyburke.subnautica.freeread.mod");
            harmony.PatchAll();
        }
    }

    [Menu("FreeRead Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("Open Journal and Auto Move")]
        public KeyCode FreeReadKey = KeyCode.RightControl;
    }
}
