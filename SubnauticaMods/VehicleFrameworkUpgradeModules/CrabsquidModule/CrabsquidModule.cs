using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
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
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Nautilus.Utility.ImageUtils.LoadSpriteFromFile(Path.Combine(modPath, "CrabsquidModuleIcon.png"));
        }

    }
}