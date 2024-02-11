using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Utility;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace FreeLook
{
    public static class Logger
    {
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo(message);
        }
        public static void Output(string msg, int x = 500, int y = 0)
        {
            BasicText message = new BasicText(x, y);
            message.ShowMessage(msg, 4);
        }
    }

    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;
        [Toggle("Enable Hinting")]
        public bool isHintingEnabled = true;
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    class FreeLookPatcher : BaseUnityPlugin
    {
        internal static MyConfig FLConfig { get; private set; }
        public static bool isFreeLooking;
        public void Start()
        {
            FreeLook.Logger.MyLog = base.Logger;

            FLConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            isFreeLooking = false;

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}
