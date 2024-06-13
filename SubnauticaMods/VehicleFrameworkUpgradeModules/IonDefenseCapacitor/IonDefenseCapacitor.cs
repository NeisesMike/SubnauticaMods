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

namespace IonDefenseCapacitor
{
    [BepInPlugin("com.mikjaw.subnautica.iondefensecapacitor.mod", "IonDefenseCapacitor", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static float invincibilityDuration = 3f;
        public static float cooldown = 45f;
        public static float energyCost = 50f;
        public static void AddIonCapacitor()
        {
            string classId = "IonDefenseCapacitor";
            //string displayName = LocalizationManager.GetString(EnglishString.thing);
            //   string description = LocalizationManager.GetString(EnglishString.Depththing);
            string displayName = "Ion Defense Capacitor";
            string description = "Temporarily envelops the sub in an ion-charged shield, nullifying all damage for a short duration.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.PrecursorIonPowerCell, 1),
                    new Tuple<TechType, int>(TechType.AdvancedWiringKit, 1),
                    new Tuple<TechType, int>(TechType.Polyaniline, 1),
                    new Tuple<TechType, int>(TechType.EnameledGlass, 2),
                    new Tuple<TechType, int>(TechType.Aerogel, 1)
                };
            void OnSelected(ModVehicle mv, int slotId)
            {
                FMODUWE.PlayOneShot("event:/tools/gravcannon/repulse", mv.PilotSeats.First().Seat.transform.position, 1f);
                IEnumerator manageInvincibility(ModVehicle thisMV)
                {
                    mv.GetComponent<LiveMixin>().invincible = true;
                    yield return new WaitForSeconds(invincibilityDuration);
                    mv.GetComponent<LiveMixin>().invincible = false;

                }
                Player.main.StartCoroutine(manageInvincibility(mv));
            }
            ModuleManager.AddSelectableModule(recipe, classId, displayName, description, OnSelected, cooldown, energyCost, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddIonCapacitor();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "IonDefenseCapacitorIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }
    }
}