using System.Collections.Generic;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Utility;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Bootstrap;

namespace GlowFix
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

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    public class GlowFixPatcher : BaseUnityPlugin
    {
        internal static List<TechType> exteriorModuleTechTypes;
        public void Start()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}

