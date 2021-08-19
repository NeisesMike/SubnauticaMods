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

namespace PersistentCreatures
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PersistentCreatures] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[PersistentCreatures] " + string.Format(format, args));
        }
    }
    public static class PersistentCreaturesPatcher
    {
        internal static PersistentCreaturesConfig Config { get; private set; }

        public static void Patch()
        {
            PersistentCreatureSimulator.Init();
            Config = OptionsPanelHandler.Main.RegisterModOptions<PersistentCreaturesConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.persistentcreatures.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Persistent Creature Options")]
    public class PersistentCreaturesConfig : ConfigFile
    {
        [Slider("Simulation Period", Min = 1f, Max = 100f)]
        public float simulationPeriod = 1f;

    }
}
