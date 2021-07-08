using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace FreeRead
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[FreeRead] " + message);
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

    [Menu("FreeRead Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Supercede PDA Pause")]
        public bool isPDAPauseSuperceded = false;
    }

    public static class FreeReadOptions
    {
        public static bool isAllowingPause = true;
    }

    public class FreeReadPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static bool isCruising = false;
        public static bool isInPDA = false;

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.freeread.mod");
            harmony.PatchAll();
        }
    }
}
