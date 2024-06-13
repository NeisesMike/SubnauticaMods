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

namespace ImpulseSpeedBooster
{
    [BepInPlugin("com.mikjaw.subnautica.impulsespeedbooster.mod", "ImpulseSpeedBooster", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static float invincibilityDuration = 3f;
        public static float maxCharge = 30f;
        public static float energyCost = 5f;
        public static void AddIonCapacitor()
        {
            string classId = "ImpulseSpeedBooster";
            //string displayName = LocalizationManager.GetString(EnglishString.thing);
            //   string description = LocalizationManager.GetString(EnglishString.Depththing);
            string displayName = "Impulse Speed Booster";
            string description = "Charge to unleash a swift propulsion burst, catapulting the sub forward with increased velocity.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.PowerCell, 2),
                    new Tuple<TechType, int>(TechType.Magnetite, 2),
                    new Tuple<TechType, int>(TechType.Lithium, 1),
                    new Tuple<TechType, int>(TechType.ComputerChip, 1),
                    new Tuple<TechType, int>(TechType.Titanium, 2)
                };
            void OnSelected(ModVehicle mv, int slotId, float charge, float slotCharge)
            {
                FMODUWE.PlayOneShot("event:/sub/seamoth/pulse", mv.PilotSeats.First().Seat.transform.position, slotCharge);
                mv.useRigidbody.AddForce(mv.transform.forward * charge * mv.useRigidbody.mass * 3, ForceMode.Impulse);
            }
            ModuleManager.AddSelectableChargeableModule(recipe, classId, displayName, description, OnSelected, maxCharge, energyCost, GetIcon(), "MVCM");
        }

        public void Start()
        {

            AddIonCapacitor();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "ImpulseSpeedBoosterIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }

    }
}