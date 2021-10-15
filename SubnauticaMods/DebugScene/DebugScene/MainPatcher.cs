﻿using System;
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
using SMLHelper.V2.Options;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;
using QModManager.API.ModLoading;
using SMLHelper.V2.Utility;


namespace DebugScene
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[DebugScene] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[DebugScene] " + string.Format(format, args));
        }

        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, 0);
            message.ShowMessage(msg, 5);
        }
    }

    [QModCore]
    public static class MainPatcher
    {
        internal static DebugSceneConfig Config { get; private set; }

        [QModPatch]
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<DebugSceneConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.debugscene.mod");
            harmony.PatchAll();
        }
    }

    [Menu("DebugScene Options")]
    public class DebugSceneConfig : ConfigFile
    {
        [Toggle("StraightToScene")]
        public bool StraightToScene = false;
    }
}
