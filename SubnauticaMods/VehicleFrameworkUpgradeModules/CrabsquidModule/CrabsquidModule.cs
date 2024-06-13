using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Collections;
using Nautilus.Options.Attributes;
using Nautilus.Options;
using Nautilus.Json;
using Nautilus.Handlers;
using Nautilus.Utility;
using BepInEx;
using BepInEx.Logging;
using VehicleFramework;
using VehicleFramework.UpgradeModules;
using HarmonyLib;

namespace CrabsquidModule
{
    [BepInPlugin("com.mikjaw.subnautica.crabsquidmodule.mod", "CrabsquidModule", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static float energyCost = 5f;
        public static void AddCrabSquidProtection()
        {
            string classId = "CrabsquidModule";
            string displayName = "Crabsquid Protection Module";
            string description = "Equip to shrug off Crabsquid EMP at a small cost of energy.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.PowerCell, 1),
                    new Tuple<TechType, int>(TechType.ComputerChip, 1),
                    new Tuple<TechType, int>(TechType.AdvancedWiringKit, 2),
                    new Tuple<TechType, int>(TechType.Titanium, 1),
            };
            void OnAdded(ModVehicle mv, List<string> currentUpgrades, int slotId, bool added)
            {
                return;
            }
            ModuleManager.AddPassiveModule(recipe, classId, displayName, description, OnAdded, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddCrabSquidProtection();
            var harmony = new Harmony("com.mikjaw.subnautica.crabsquidmodule.mod");
            harmony.PatchAll();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "CrabsquidModuleIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }

    }
}