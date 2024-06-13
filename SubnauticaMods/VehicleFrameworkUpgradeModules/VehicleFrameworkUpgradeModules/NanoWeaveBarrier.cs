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

namespace NanoWeaveBarrier
{
    [BepInPlugin("com.mikjaw.subnautica.nanoweavebarrier.mod", "NanoWeaveBarrier", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static void AddNormalDamageModifier()
        {
            string classId = "NanoWeaveBarrier";
            //string displayName = LocalizationManager.GetString(EnglishString.thing);
            //   string description = LocalizationManager.GetString(EnglishString.Depththing);
            string displayName = "Nano-Weave Barrier";
            string description = "Employs nanotechnology to weave an ultra-durable mesh, reinforcing the sub's exterior against environmental hazards.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>()
                {
                    new Tuple<TechType, int>(TechType.AramidFibers, 2),
                    new Tuple<TechType, int>(TechType.Diamond, 1),
                    new Tuple<TechType, int>(TechType.PlasteelIngot, 1),
                    new Tuple<TechType, int>(TechType.CopperWire, 1),
                    new Tuple<TechType, int>(TechType.EnameledGlass, 1)
                };
            void OnAdded(ModVehicle mv, List<string> currentUpgrades, int slotId, bool added)
            {
                var damg = mv.gameObject.EnsureComponent<DamageModifier>();
                damg.damageType = DamageType.Normal;
                damg.multiplier = Mathf.Pow(0.90f, currentUpgrades.Where(x => x.Contains("NanoWeaveBarrier")).Count());
            }
            ModuleManager.AddPassiveModule(recipe, classId, displayName, description, OnAdded, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddNormalDamageModifier();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "NanoWeaveBarrierIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }
    }
}