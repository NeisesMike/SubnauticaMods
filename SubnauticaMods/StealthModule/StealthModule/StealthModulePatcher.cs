using UnityEngine;
using HarmonyLib;

namespace StealthModule
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[StealthModule] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[StealthModule] " + string.Format(format, args));
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

    public class StealthModulePatcher
    {
        internal static StealthQuality stealthQuality = StealthQuality.None;

        internal static SeamothStealthModule1 stealthModule1 = new SeamothStealthModule1();
        internal static SeamothStealthModule2 stealthModule2 = new SeamothStealthModule2();
        internal static SeamothStealthModule3 stealthModule3 = new SeamothStealthModule3();

        public static void Patch()
        {
            stealthModule1.Patch();
            stealthModule2.Patch();
            stealthModule3.Patch();

            var harmony = new Harmony("com.mikjaw.subnautica.stealthmodule.mod");
            harmony.PatchAll();
        }
    }

    public enum StealthQuality
    {
        None,
        Low,
        Medium,
        High,
        Debug
    }
}
