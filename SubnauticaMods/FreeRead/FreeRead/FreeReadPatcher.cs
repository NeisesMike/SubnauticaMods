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
using Nautilus.Options;
using Nautilus.Utility;


using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;


namespace FreeRead
{
    [BepInPlugin("com.mikjaw.subnautica.freeread.mod", "FreeRead", "2.0.2")]
    [BepInDependency("com.snmodding.nautilus")]
    public class FreeReadPatcher : BaseUnityPlugin
    {
        public static bool isCruising = false;
        internal static Config config { get; private set; }


        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<Config>();
            var harmony = new Harmony("com.mikjaw.subnautica.freeread.mod");
            harmony.PatchAll();
            //Logger.LogWarning("done");
        }
    }

    [Menu("FreeRead Options")]
    public class Config : ConfigFile
    {
        [Keybind("Open Journal and Auto Move")]
        public KeyCode FreeReadKey = KeyCode.RightControl;
    }
}
