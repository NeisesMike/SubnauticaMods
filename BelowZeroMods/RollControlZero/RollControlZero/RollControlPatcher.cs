using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;

namespace RollControlZero
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[RollControlZero] " + message);
        }
        public static void Output(string msg)
        {
            Hint main = Hint.main;
            if (main == null)
            {
                return;
            }
            uGUI_PopupMessage message = main.message;
            message.ox = 60f;
            message.oy = 0f;
            message.anchor = TextAnchor.MiddleRight;
            message.SetBackgroundColor(new Color(1f, 1f, 1f, 1f));
            string myMessage = msg;
            message.SetText(myMessage, TextAnchor.MiddleRight);
            message.Show(3f, 0f, 0.25f, 0.25f, null);
        }
    }

    [Menu("RollControl Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Roll Enabled")]
        public bool isSeatruckRollOn = true;
        [Keybind("Roll Toggle")]
        public KeyCode rollToggleKey = KeyCode.AltGr;
        [Keybind("Roll To Port")]
        public KeyCode rollToPortKey = KeyCode.Z;
        [Keybind("Roll To Starboard")]
        public KeyCode rollToStarboardKey = KeyCode.C;
        [Slider("Roll Speed Slider", 1, 30, DefaultValue = 15)]
        public float seatruckRollSpeed = 15;
        [Toggle("Roll HUD Element")]
        public bool isHUD = true;
        [Choice("Roll HUD Placement")]
        public TextAnchor HUDAnchor = TextAnchor.LowerRight;
    }

    public class RollControlPatcher
    {
        internal static MyConfig Config { get; private set; }

        //public static bool isScubaRollOn = false;
        //public static RollManager myRollMan = new RollManager();

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrolzero.mod");
            harmony.PatchAll();
        }
    }
}

