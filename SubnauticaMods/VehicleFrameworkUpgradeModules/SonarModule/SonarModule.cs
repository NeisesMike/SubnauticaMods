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

namespace SonarModule
{
    [BepInPlugin("com.mikjaw.subnautica.sonarmodule.mod", "SonarModule", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static float invincibilityDuration = 3f;
        public static float cooldown = 45f;
        public static void AddIonCapacitor()
        {
            string classId = "SonarModule";
            //string displayName = LocalizationManager.GetString(EnglishString.thing);
            //   string description = LocalizationManager.GetString(EnglishString.Depththing);
            string displayName = "Sonar Module";
            string description = "A dedicated system for detecting and displaying topographical data on the HUD.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.ComputerChip, 1),
                    new Tuple<TechType, int>(TechType.Magnetite, 2),
                    new Tuple<TechType, int>(TechType.Titanium, 1),
                    new Tuple<TechType, int>(TechType.Glass, 1)
                };
            void OnToggle(ModVehicle mv, int slotId)
            {
                SNCameraRoot.main.SonarPing();
                FMODUWE.PlayOneShot("event:/sub/seamoth/sonar_loop", mv.PilotSeats.First().Seat.transform.position, 1f);
            }
            ModuleManager.AddToggleableModule(recipe, classId, displayName, description, OnToggle, energyCostPerActivation: 1f, timeToFirstActivation: 0f, repeatRate: 5f, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddIonCapacitor();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "SonarModuleIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }
    }
}