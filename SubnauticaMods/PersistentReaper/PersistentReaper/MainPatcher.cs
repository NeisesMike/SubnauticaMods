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
using Nautilus.Options;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace PersistentReaper
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static ManualLogSource PRLogger { get; private set; }
        internal static MyConfig PRConfig { get; private set; }

        public void Start()
        {
            PRLogger = base.Logger;
            PRConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}
