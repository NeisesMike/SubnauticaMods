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

namespace PassiveGhostLeviathans
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PassiveGhostLeviathans] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[PassiveGhostLeviathans] " + string.Format(format, args));
        }
    }
    public class PassiveGhostLeviathansPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.garyburke.subnautica.passiveghostleviathans.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Passive Ghost Leviathan Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Passive Ghost Leviathans")]
        public bool isGhostPassive = true;

        [Toggle("Ghost Leviathans No Bite Damage")]
        public bool isNoBiteDamage = false;
    }
}
