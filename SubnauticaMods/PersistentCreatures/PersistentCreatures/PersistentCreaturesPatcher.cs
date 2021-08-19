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
using SMLHelper.V2.Utility;

namespace PersistentCreatures
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PersistentCreatures] " + message);
        }

        public static void output(string msg)
        {
            BasicText message = new BasicText(250, 250);
            message.ShowMessage(msg, 1);
        }
    }
    public static class PersistentCreaturesPatcher
    {
        internal static PersistentCreaturesConfig Config { get; private set; }
        internal static PersistentCreatureSimulator Simulator { get; set; }
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<PersistentCreaturesConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.persistentcreatures.mod");
            harmony.PatchAll();
        }
    }

    [Menu("Persistent Creature Options")]
    public class PersistentCreaturesConfig : ConfigFile
    {
        [Slider("Simulation Period", Min = 1f, Max = 100f, DefaultValue = 10f)]
        public float simulationPeriod = 1f;

        [Slider("Creatures per Thread", Min = 1, Max = 100, DefaultValue = 50)]
        public int creaturesPerTask = 50;

        [Button("Count Persistent Creatures")]
        public static void printPCCount()
        {
            if (PersistentCreaturesPatcher.Simulator != null)
            {
                Logger.output("There are currently " + PersistentCreatureSimulator.getCreatures().Count.ToString() + " persistent creatures.");
            }
        }
    }
}
