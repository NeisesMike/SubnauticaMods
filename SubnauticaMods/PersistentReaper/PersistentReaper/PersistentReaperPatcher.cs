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

namespace PersistentReaper
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PersistentReapers] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[PersistentReapers] " + string.Format(format, args));
        }
    }
    public static class PersistentReaperPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.persistentreapers.mod");
            harmony.PatchAll();
        }
    }

    public enum ReaperBehaviors
    {
        Normal,
        Bloodthirsty,
        HumanHunter
    }

    public enum DepthMap
    {
        Normal,
        NoShallowReapers
    }

    [Menu("Persistent Reaper Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Toggle Persistent Reapers"), OnChange(nameof(killAllReapers))]
        public bool areReapersActive = true;
        public void killAllReapers()
        {
            if (ReaperManager.reaperDict != null)
            {
                int numCurrentReapers = ReaperManager.reaperDict.Count;
                for (int i = 0; i < numCurrentReapers; i++)
                {
                    ReaperManager.removeOneReaper();
                }
            }
        }

        [Slider("Number of Persistent Reapers", Min = 0, Max = 250, Step = 5, DefaultValue = 50)]
        public int numReapers = 50;

        [Choice("Persistent Reaper Behavior")]
        public ReaperBehaviors reaperBehaviors = ReaperBehaviors.Normal;

        public const string scentLifetimeTooltip = "Change how long your scent sticks around. Higher numbers make you easier to track. Consider choosing a lower number if performance is suffering.";
        [Slider("Scent Lifetime (Seconds)", Min = 0, Max = 600, Step = 5, DefaultValue = 120, Tooltip = scentLifetimeTooltip)]
        public int scentLifetime = 120;

        [Choice("Depth Map"), OnChange(nameof(setDepthMap))]
        public DepthMap depthMapChoice = DepthMap.NoShallowReapers;

        public void setDepthMap(ChoiceChangedEventArgs e)
        {
            ReaperManager.depthDictionary = ReaperManager.getDepthDictionary(depthMapChoice);
            killAllReapers();
        }

        public const string updateIntervalTooltip = "Change this to make the reapers simulate less frequently. Simulation is balanced for 1 second here, so only increase this number if performance is suffering.";
        [Slider("Reaper Update Interval (seconds)", Min = 1f, Max = 10f, Step = 0.25f, DefaultValue = 1f, Tooltip = updateIntervalTooltip)]
        public float updateInterval = 1f;

        [Button("Print Reaper Map")]
        public static void printReaperMap()
        {
            string[] map = new string[256];

            for (int x = 0; x < 256; x++)
            {
                string mapRow = "";
                for (int z = 0; z < 256; z++)
                {
                    mapRow += '.';
                }
                map[x] = mapRow;
            }

            foreach (KeyValuePair<ReaperBehavior, GameObject> entry in ReaperManager.reaperDict)
            {
                Int3 __instance3Loc = entry.Key.currentRegion;
                StringBuilder sb = new StringBuilder(map[__instance3Loc.x]);
                sb[__instance3Loc.z] = 'X';
                map[__instance3Loc.x] = sb.ToString();
            }

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllLines(Path.Combine(modPath, "ReaperMap.txt"), map);
        }
    }
}
