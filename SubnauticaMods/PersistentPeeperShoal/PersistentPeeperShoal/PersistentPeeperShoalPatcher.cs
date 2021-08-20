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
using QModManager.API.ModLoading;

namespace PersistentPeeperShoal
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[PersistentPeeperShoal] " + message);
        }

        public static void output(string msg)
        {
            BasicText message = new BasicText(250, 250);
            message.ShowMessage(msg, 1);
        }
    }

    [QModCore]
    public static class PersistentPeeperShoalPatcher
    {
        internal static PersistentPeeperShoalConfig Config { get; private set; }

        internal static GameObject Prefab { get; set; }

        [QModPatch]
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<PersistentPeeperShoalConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.persistentpeepershoal.mod");
            harmony.PatchAll();
            ResetShoals();
        }

        public static void ResetShoals()
        {
            // register all our shoals
            for (int i = 0; i < Config.numShoals; i++)
            {
                PeeperShoal thisShoal = new PeeperShoal();
                Int3 start = PersistentCreatures.Utils.GetRandomStartLocation();
                thisShoal.Register(start.x, start.y, start.z);
            }
        }
    }

    [Menu("Persistent Peeper Shoal Options")]
    public class PersistentPeeperShoalConfig : ConfigFile
    {
        [Slider("Number of Shoals", Min = 1, Max = 100, DefaultValue = 50)]
        public int numShoals = 1000;

        [Slider("Min Peepers per Shoal", Min = 1, Max = 100, DefaultValue = 10)]
        public int minPeepers = 100;

        [Slider("Max Peepers per Shoal", Min = 1, Max = 500, DefaultValue = 30)]
        public int maxPeepers = 100;

        [Slider("angle increment", Min = 0.01f, Max = 10f, Step = 0.01f, DefaultValue = 1f)]
        public float angleIncrement = 1f;
        [Slider("cycle update rate", Min = 0.01f, Max = 2f, Step = 0.01f)]
        public float cycleUpdateRate = 1f;
        [Slider("geo scale", Min = 0f, Max = 5f, Step = 0.1f)]
        public float geoScale = 1f;
        [Slider("cycle height", Min = 1f, Max = 30f, Step = 0.5f)]
        public float cycleHeight = 1f;
        [Slider("a", Min = 1f, Max = 10f, Step = 0.1f)]
        public float a = 1f;
        [Slider("b", Min = 1f, Max = 10f, Step = 0.1f)]
        public float b = 1f;
    }
}
