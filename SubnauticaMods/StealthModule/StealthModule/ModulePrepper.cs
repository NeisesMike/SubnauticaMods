using System;
using System.Collections.Generic;
using UnityEngine;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using BiomeData = LootDistributionData.BiomeData;

namespace StealthModule
{
    static class ModulePrepper
    {
        private static TechType RegisterSeamothModuleGeneric(List<CraftData.Ingredient> recipe, string classId, string displayName, string description)
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
            module_CustomPrefab.SetRecipe(modulerRecipe).WithCraftingTime(3).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab("SeamothModules");
            module_CustomPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
            module_CustomPrefab.SetUnlock(TechType.BaseUpgradeConsole);
            //module_CustomPrefab.SetEquipment(EquipmentType.SeamothModule);
            module_CustomPrefab.SetVehicleUpgradeModule(equipmentType: EquipmentType.SeamothModule);
            module_CustomPrefab.Register();
            return module_info.TechType;
        }

        public static TechType RegisterSeamoth1()
        {
            string classId = "SeamothStealthModule1";
            string displayName = "Seamoth Stealth Module Mk 1";
            string description = "Presence masking past 80 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.FiberMesh, 1),
                    new CraftData.Ingredient(TechType.Quartz, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterSeamothModuleGeneric(recipe, classId, displayName, description);
        }
        public static TechType RegisterSeamoth2(TechType seamoth1TT)
        {
            string classId = "SeamothStealthModule2";
            string displayName = "Seamoth Stealth Module Mk 2";
            string description = "Presence masking past 60 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(seamoth1TT, 1),
                    new CraftData.Ingredient(TechType.Silicone, 1),
                    new CraftData.Ingredient(TechType.Lithium, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterSeamothModuleGeneric(recipe, classId, displayName, description);
        }
        public static TechType RegisterSeamoth3(TechType seamoth2TT)
        {
            string classId = "SeamothStealthModule3";
            string displayName = "Seamoth Stealth Module Mk 3";
            string description = "Presence masking past 40 meters. Does not stack.";
            List<CraftData.Ingredient> recipe = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(seamoth2TT, 1),
                    new CraftData.Ingredient(TechType.AramidFibers, 1),
                    new CraftData.Ingredient(TechType.Nickel, 1),
                    new CraftData.Ingredient(TechType.Gold, 1)
                };
            return RegisterSeamothModuleGeneric(recipe, classId, displayName, description);
        }

    }
}
