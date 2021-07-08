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
using SMLHelper.V2.Utility;
using LitJson;
using System.Net.NetworkInformation;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;

namespace FreeLook
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeLookZero] " + message);
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

    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;
        [Toggle("Enable Hinting")]
        public bool isHintingEnabled = true;
    }

    class FreeLookPatcher
    {
        internal static MyConfig Config { get; private set; }
        public static bool isFreeLooking;
        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            isFreeLooking = false;

            var harmony = new Harmony("com.garyburke.subnautica.freelookzero.mod");
            harmony.PatchAll();
        }
    }
}
