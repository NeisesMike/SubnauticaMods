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
using Nautilus.Utility;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;

#if BELOWZERO
using QModManager.API.ModLoading;
#endif

namespace InsertGameObject
{
#if SUBNAUTICA
    [BepInPlugin("com.mikjaw.subnautica.insertgameobject.mod", "InsertGameObject", "1.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static InsertGameObjectConfig config { get; private set; }
        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<InsertGameObjectConfig>();
            InsertGameObject.Logger.MyLog = base.Logger;
            var harmony = new Harmony("com.mikjaw.subnautica.insertgameobject.mod");
            harmony.PatchAll();
        }
    }
#endif

#if BELOWZERO
    [QModCore]
    public class MainPatcher
    {
        internal static InsertGameObjectConfig config { get; private set; }

        [QModPatch]
        public static void Patch()
        {
            config = OptionsPanelHandler.Main.RegisterModOptions<InsertGameObjectConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.insertgameobject.mod");
            harmony.PatchAll();
        }
    }
#endif

    [Menu("InsertGameObject Options")]
    public class InsertGameObjectConfig : ConfigFile
    {
        [Keybind("Spawn GameObject"), Tooltip("The GameObject will be spawned [Spawn Distance] meters directly in front of you.")]
        public KeyCode SpawnKey = KeyCode.RightAlt;

        [Slider("Spawn Distance", Min = 1f, Max = 25f, Step = 1f, DefaultValue = 10f), Tooltip("This is how far away from you the GameObject will be when you spawn it.")]
        public float SpawnDistance = 10f;

        [Slider("Size Multiplier", Min = 0f, Max = 10f, Step = .1f, DefaultValue = 1f), Tooltip("This is how large the GameObject will be compared to normal.")]
        public float SizeMultiplier = 1f;
    }

    public static class Logger
    {
        public static ManualLogSource MyLog { get; set; }
        public static void Warn(string message)
        {
            MyLog.LogWarning("[InsertGameObject] " + message);
        }
        public static void Log(string message)
        {
            MyLog.LogInfo("[InsertGameObject] " + message);
        }
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, 0);
            message.ShowMessage(msg, 5);
        }
    }

}
