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
using System.Runtime.CompilerServices;
using System.Collections;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Utility;
using Nautilus.Patchers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace AttitudeIndicator
{
    public static class AILogger
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


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    public class AttitudeIndicatorPatcher : BaseUnityPlugin
    {
        internal static SubnauticaConfig SubnauticaConfig { get; private set; }
        public void Start()
        {
            AILogger.MyLog = base.Logger;
            SubnauticaConfig = OptionsPanelHandler.RegisterModOptions<SubnauticaConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}

