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

namespace SeamothEject
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[SeamothEject] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[SeamothEject] " + string.Format(format, args));
        }
    }
    public class SeamothEjectPatcher
    {
        internal static MyConfig Config { get; private set; }
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.garyburke.subnautica.seamotheject.mod");
            harmony.PatchAll();
        }
    }
    
    public enum EjectionPlacement
    {
        Behind,
        Above,
        Left,
        Right,
        Below,
        Front,
        Normal
    }

    [Menu("SeamothEject Options")]
    public class MyConfig : ConfigFile
    {
        [Choice("Eject Where")]
        public EjectionPlacement myPlacement = EjectionPlacement.Behind;
    }
}
