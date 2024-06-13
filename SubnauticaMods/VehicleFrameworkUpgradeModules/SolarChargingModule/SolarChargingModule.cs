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

namespace SolarChargingModule
{
    [BepInPlugin("com.mikjaw.subnautica.solarchargingmodule.mod", "SolarChargingModule", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static void AddSolarChargingModule()
        {
            string classId = "SolarChargingModule";
            //string displayName = LocalizationManager.GetString(EnglishString.thing);
            //   string description = LocalizationManager.GetString(EnglishString.Depththing);
            string displayName = "Solar Charging Module";
            string description = "Recharge your ship's batteries with solar energy during daylight; effectiveness increases with sun elevation and proximity to the surface.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.AdvancedWiringKit, 2),
                    new Tuple<TechType, int>(TechType.Titanium, 3),
                    new Tuple<TechType, int>(TechType.EnameledGlass, 2)
                };
            void OnAdded(ModVehicle mv, List<string> currentUpgrades, int slotId, bool added)
            {
                mv.gameObject.EnsureComponent<VFSolarCharger>().numChargers = currentUpgrades.Where(x => x.Contains("SolarChargingModule")).Count();
            }
            ModuleManager.AddPassiveModule(recipe, classId, displayName, description, OnAdded, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddSolarChargingModule();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "SolarChargingModuleIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }
    }
}