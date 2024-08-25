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

namespace DroneRange
{
    [BepInPlugin("com.mikjaw.subnautica.dronerange.mod", "DroneRange", "1.0")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", MinimumDependencyVersion: "1.2.8")]
    public class MainPatcher : BaseUnityPlugin
    {
        public static void AddDroneRangeUpgrade()
        {
            string classId = "DroneRangeModule";
            string displayName = "Drone Range Upgrade";
            string description = "Boosts the effective operating range of drones by 200 meters. Stacks.";
            List<Tuple<TechType, int>> recipe = new List<Tuple<TechType, int>>() // adjust recipe
                {
                    new Tuple<TechType, int>(TechType.AdvancedWiringKit, 1),
                    new Tuple<TechType, int>(TechType.ComputerChip, 1),
                };
            void OnAdded(ModVehicle mv, List<string> currentUpgrades, int slotId, bool added)
            {
                VehicleFramework.VehicleTypes.Drone drone = mv as VehicleFramework.VehicleTypes.Drone;
                if(drone != null)
                {
                    drone.addedConnectionDistance = 200 * currentUpgrades.Where(x => x.Contains(classId)).Count();
                }
                else
                {
                    if (added)
                    {
                        VehicleFramework.Logger.Output("This upgrade has no effect on this vehicle.");
                    }
                }
            }
            ModuleManager.AddPassiveModule(recipe, classId, displayName, description, OnAdded, GetIcon(), "MVCM");
        }

        public void Start()
        {
            AddDroneRangeUpgrade();
        }

        public static Atlas.Sprite GetIcon()
        {
            // grab the icon image
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            byte[] spriteBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "DroneRangeIcon.png"));
            Texture2D SpriteTexture = new Texture2D(128, 128);
            SpriteTexture.LoadImage(spriteBytes);
            Sprite mySprite = Sprite.Create(SpriteTexture, new Rect(0.0f, 0.0f, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return new Atlas.Sprite(mySprite);
        }
    }
}