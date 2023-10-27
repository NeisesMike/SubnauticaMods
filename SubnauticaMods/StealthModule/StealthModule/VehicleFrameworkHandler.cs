using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using BiomeData = LootDistributionData.BiomeData;

namespace StealthModule
{
    public static class VehicleFrameworkHandler
    {
        public static TechType mv1;
        public static TechType mv2;
        public static TechType mv3;
        public static void PatchModVehicleModules(ref Harmony harmony)
        {

            // patch in the crafting node for the vehicle upgrade menu
            string[] stepsToStealthTab = { "MVUM" };
            Nautilus.Handlers.CraftTreeHandler.AddTabNode(CraftTree.Type.SeamothUpgrades, "MVSM", "ModVehicle Stealth Modules", StealthModulePatcher.stealthSpriteAtlas, stepsToStealthTab);

            mv1 = RegisterModVehicle1();
            mv2 = RegisterModVehicle2(mv1);
            mv3 = RegisterModVehicle3(mv2);

            var ModVehicleType = Type.GetType("VehicleFramework.ModVehicle, VehicleFramework", false, false);

            var AwakeMethod = AccessTools.Method(ModVehicleType, "Awake");
            var AwakePostfix = new HarmonyMethod(AccessTools.Method(typeof(MVAwakePatcher), "Postfix"));
            harmony.Patch(AwakeMethod, AwakePostfix);

            var OnUpModChangeMethod = AccessTools.Method(ModVehicleType, "OnUpgradeModuleChange");
            var OnUpMCPostfix = new HarmonyMethod(AccessTools.Method(typeof(MVOnUpgradeModuleChangePatcher), "Postfix"));
            harmony.Patch(OnUpModChangeMethod, OnUpMCPostfix);
        }

        private static TechType RegisterModVehicleModuleGeneric(List<CraftData.Ingredient> recipe, string classId, string displayName, string description)
        {
            Nautilus.Crafting.RecipeData modulerRecipe = new Nautilus.Crafting.RecipeData();
            modulerRecipe.Ingredients.AddRange(recipe);
            PrefabInfo module_info = PrefabInfo.WithTechType(classId, displayName, description, unlockAtStart: false);
            module_info.WithIcon(StealthModulePatcher.stealthSpriteAtlas);
            CustomPrefab module_CustomPrefab = new CustomPrefab(module_info);
            PrefabTemplate moduleTemplate = new CloneTemplate(module_info, TechType.SeamothElectricalDefense)
            {
                ModifyPrefab = prefab => prefab.GetComponentsInChildren<Renderer>().ForEach(r => r.materials.ForEach(m => m.color = Color.blue))
            };
            module_CustomPrefab.SetGameObject(moduleTemplate);
            module_CustomPrefab.SetRecipe(modulerRecipe).WithCraftingTime(3).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab(new string[] { "MVUM", "MVSM" });
            module_CustomPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
            module_CustomPrefab.SetUnlock(TechType.BaseUpgradeConsole);
            module_CustomPrefab.SetEquipment(VehicleFramework.VehicleBuilder.ModuleType);
            module_CustomPrefab.Register();
            return module_info.TechType;
        }
        public static TechType RegisterModVehicle1()
        {
            string classId = "ModVehicleStealthModule1";
            string displayName = "Vehicle Stealth Module Mk 1";
            string description = "Presence masking past 80 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.FiberMesh, 1),
                    new CraftData.Ingredient(TechType.Quartz, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterModVehicleModuleGeneric(recipe, classId, displayName, description);
        }
        public static TechType RegisterModVehicle2(TechType seamoth1TT)
        {
            string classId = "ModVehicleStealthModule2";
            string displayName = "Vehicle Stealth Module Mk 2";
            string description = "Presence masking past 60 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(seamoth1TT, 1),
                    new CraftData.Ingredient(TechType.Silicone, 1),
                    new CraftData.Ingredient(TechType.Lithium, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterModVehicleModuleGeneric(recipe, classId, displayName, description);
        }
        public static TechType RegisterModVehicle3(TechType seamoth2TT)
        {
            string classId = "ModVehicleStealthModule3";
            string displayName = "Vehicle Stealth Module Mk 3";
            string description = "Presence masking past 40 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(seamoth2TT, 1),
                    new CraftData.Ingredient(TechType.AramidFibers, 1),
                    new CraftData.Ingredient(TechType.Nickel, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterModVehicleModuleGeneric(recipe, classId, displayName, description);
        }


    }






}
