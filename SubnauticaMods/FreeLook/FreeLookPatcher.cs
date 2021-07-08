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
using SMLHelper.V2.Utility;
using LitJson;
using System.Net.NetworkInformation;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;

namespace FreeLook
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeLook] " + message);
        }
    }

    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Enable Hinting")]
        public bool isHintingEnabled = true;

        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;
    }

    class FreeLookPatcher
    {
        internal static MyConfig Config { get; private set; }
        //public static Options Options = new Options();
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            //OptionsPanelHandler.RegisterModOptions(Options);
            var harmony = new Harmony("com.mikjaw.subnautica.freelook.mod");
            harmony.PatchAll();
        }
    }
}
