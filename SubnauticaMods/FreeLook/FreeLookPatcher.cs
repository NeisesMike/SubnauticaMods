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

            // here we coerce tweaks and fixes into compatibility
            // in other words, we neuter one of its patches.
            var type2 = Type.GetType("Tweaks_Fixes.SeaMoth_patch, Tweaks and Fixes", false, false);
            if (type2 != null)
            {
                var TweaksFixesSeamothUpdatePrefix = AccessTools.Method(type2, "UpdatePrefix");
                harmony.Patch(TweaksFixesSeamothUpdatePrefix, prefix: new HarmonyMethod(typeof(FreeLookPatcher), nameof(TweaksFixesSeamothUpdatePrefixPrefix)));
            }
        }

        public static bool TweaksFixesSeamothUpdatePrefixPrefix(SeaMoth __instance, ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
