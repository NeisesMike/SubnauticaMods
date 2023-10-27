using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using BiomeData = LootDistributionData.BiomeData;
using System.Collections.Generic;

namespace StealthModule
{ 
    public class SubLog
    { 
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

    [BepInPlugin("com.mikjaw.subnautica.stealthmodule.mod", "StealthModule", "2.0.2")]
    [BepInDependency("com.snmodding.nautilus")]
    public class StealthModulePatcher : BaseUnityPlugin
    {
        public static ManualLogSource logger { get; private set; }
        internal static StealthQuality stealthQuality = StealthQuality.None;

        internal static Atlas.Sprite stealthSpriteAtlas;

        /*
        internal static SeamothStealthModule1 seamothStealthModule1 = new SeamothStealthModule1();
        internal static SeamothStealthModule2 seamothStealthModule2 = new SeamothStealthModule2();
        internal static SeamothStealthModule3 seamothStealthModule3 = new SeamothStealthModule3();
        */
        internal static MyConfig config { get; private set; }

        internal static TechType seamoth1;
        internal static TechType seamoth2;
        internal static TechType seamoth3;

        public void Start()
        {
            logger = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/SeamothStealthModuleIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            stealthSpriteAtlas = new Atlas.Sprite(mySprite);

            seamoth1 = ModulePrepper.RegisterSeamoth1();
            seamoth2 = ModulePrepper.RegisterSeamoth2(seamoth1);
            seamoth3 = ModulePrepper.RegisterSeamoth3(seamoth2);
            /*
            seamothStealthModule1.Patch();
            seamothStealthModule2.Patch();
            seamothStealthModule3.Patch();
            */

            var harmony = new Harmony("com.mikjaw.subnautica.stealthmodule.mod");
            var type = System.Type.GetType("VehicleFramework.ModVehicle, VehicleFramework", false, false);
            if (type != null)
            {
                VehicleFrameworkHandler.PatchModVehicleModules(ref harmony);
            }

            harmony.PatchAll();
        }

        [Menu("Stealth Module Options")]
        public class MyConfig : ConfigFile
        {
            [Toggle("Enable Leviathan Distance Indicator")]
            public bool isDistanceIndicatorEnabled = true;
        }
    }

}
