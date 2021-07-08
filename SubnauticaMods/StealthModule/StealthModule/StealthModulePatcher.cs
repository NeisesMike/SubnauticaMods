using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;

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

        internal static Atlas.Sprite stealthSpriteAtlas;

        internal static SeamothStealthModule1 stealthModule1 = new SeamothStealthModule1();
        internal static SeamothStealthModule2 stealthModule2 = new SeamothStealthModule2();
        internal static SeamothStealthModule3 stealthModule3 = new SeamothStealthModule3();

        public static void Patch()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/SeamothStealthModuleIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            stealthSpriteAtlas = new Atlas.Sprite(mySprite);

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
