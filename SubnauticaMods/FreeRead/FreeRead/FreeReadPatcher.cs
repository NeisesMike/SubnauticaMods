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
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;


using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;


namespace FreeRead
{
    [BepInPlugin("com.mikjaw.subnautica.freeread.mod", "FreeRead", "1.0")]
    public class FreeReadPatcher : BaseUnityPlugin
    {
        public static bool isCruising = false;
        internal static Config config { get; private set; }


        public void Start()
        {
            config = OptionsPanelHandler.Main.RegisterModOptions<Config>();
            var harmony = new Harmony("com.garyburke.subnautica.freeread.mod");
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
