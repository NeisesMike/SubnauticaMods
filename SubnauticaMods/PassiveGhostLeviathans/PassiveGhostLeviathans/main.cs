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
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace PassiveGhostLeviathans
{
    [BepInPlugin("com.mikjaw.subnautica.freeread.mod", "FreeRead", "1.0")]
    public class PassiveGhostLeviathansPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }

        public void Start()
        {
            config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.garyburke.subnautica.passiveghostleviathans.mod");
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
