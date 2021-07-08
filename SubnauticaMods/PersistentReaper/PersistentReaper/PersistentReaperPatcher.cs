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
    public class PersistentReaperPatcher
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
        [Toggle("Toggle Persistent Reapers")]
        public bool areReapersActive = true;

        [Slider("Number of Persistent Reapers", Min = 0, Max = 250, Step = 5, DefaultValue = 50)]
        public int numReapers = 50;

        [Choice("Persistent Reaper Behavior")]
        public ReaperBehaviors reaperBehaviors = ReaperBehaviors.Normal;

        [Slider("Scent Lifetime (Seconds)", Min = 0, Max = 600, Step = 5, DefaultValue = 120)]
        public int scentLifetime = 120;

        [Choice("Depth Map"), OnChange(nameof(setDepthMap))]
        public DepthMap depthMapChoice = DepthMap.NoShallowReapers;

        public static void setDepthMap()
        {
            ReaperManager.depthDictionary = ReaperManager.getDepthDictionary();
        }

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

            foreach (KeyValuePair<GameObject, ReaperBehavior> entry in ReaperManager.reaperDict)
            {
                Int3 __instance3Loc = entry.Value.currentRegion;
                StringBuilder sb = new StringBuilder(map[__instance3Loc.x]);
                sb[__instance3Loc.z] = 'X';
                map[__instance3Loc.x] = sb.ToString();
            }

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllLines(Path.Combine(modPath, "ReaperMap.txt"), map);
        }
    }
}
