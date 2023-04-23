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

namespace SeamothEject
{
    [BepInPlugin("com.mikjaw.subnautica.seamotheject.mod", "SeamothEject", "1.0")]
    public class SeamothEjectPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        public void Start()
        {
            config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
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
