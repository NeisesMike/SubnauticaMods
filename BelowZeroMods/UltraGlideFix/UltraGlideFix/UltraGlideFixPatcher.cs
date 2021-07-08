using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace UltraGlideFix
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[UltraGlideFix] " + message);
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

        public static float UpdateAcceleration(float inputAccel)
        {
            return inputAccel * 1.5f;
        }
        
    }

    public class UltraGlideFixPatcher
    {
        public static void Patch()
        {
            var harmony = new Harmony("com.mikjaw.subnautica.ultraglidefix.mod");
            harmony.PatchAll();
        }
    }
}
