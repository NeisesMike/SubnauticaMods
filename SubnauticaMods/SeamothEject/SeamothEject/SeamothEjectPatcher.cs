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

namespace SeamothEject
{
    [BepInPlugin("com.mikjaw.subnautica.seamotheject.mod", "SeamothEject", "2.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class SeamothEjectPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.seamotheject.mod");
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
