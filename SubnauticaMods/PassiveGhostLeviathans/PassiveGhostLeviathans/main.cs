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

namespace PassiveGhostLeviathans
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PassiveGhostLeviathans] " + message);
        }
    }

    [BepInPlugin("com.mikjaw.subnautica.passiveghostleviathans.mod", "PassiveGhostLeviathans", "2.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class PassiveGhostLeviathansPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.passiveghostleviathans.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Passive Ghost Leviathan Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Adult Ghosts Passive")]
        public bool isGhostPassive = true;

        [Toggle("Adult Ghosts Disable Bite Damage")]
        public bool isNoBiteDamage = false;

        [Toggle("Juvenile Ghosts Passive")]
        public bool isJuvenileGhostPassive = true;

        [Toggle("Juvenile Ghosts Disable Bite Damage")]
        public bool isJuvenileNoBiteDamage = false;
    }
}
