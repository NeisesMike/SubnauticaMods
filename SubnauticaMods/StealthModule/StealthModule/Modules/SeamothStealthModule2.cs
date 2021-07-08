using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.Collections.Generic;
using UnityEngine;

namespace StealthModule
{
    public class SeamothStealthModule2: Equipable
    {
        public SeamothStealthModule2() : base(
            classId: "SeamothStealthModule2",
            friendlyName: "Seamoth Stealth Module MK2",
            description: "Better presence masking. Does not stack.")
        {

        }

        public override EquipmentType EquipmentType => EquipmentType.SeamothModule;

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;

        public override string[] StepsToFabricatorTab => new string[] { "SeamothMenu", "Stealth" };

        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;

        public override GameObject GetGameObject()
        {
            // Get the ElectricalDefense module prefab and instantiate it
            string path = "WorldEntities/Tools/SeamothElectricalDefense";
            GameObject prefab = Resources.Load<GameObject>(path);
            GameObject obj = GameObject.Instantiate(prefab);

            // Get the TechTags and PrefabIdentifiers
            TechTag techTag = obj.GetComponent<TechTag>();
            PrefabIdentifier prefabIdentifier = obj.GetComponent<PrefabIdentifier>();

            // Change them so they fit to our requirements.
            techTag.type = TechType;
            prefabIdentifier.ClassId = ClassID;

            return obj;
        }
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(StealthModulePatcher.stealthModule1.TechType, 1),
                    new Ingredient(TechType.Silicone, 1),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Gold, 1)
                },
                craftAmount = 1
            };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return StealthModulePatcher.stealthSpriteAtlas;
        }
    }
}
