using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Reflection;

using UnityEngine.Sprites;


namespace StealthModule
{
    public class ModVehicleStealthModule1 : Equipable
    {
        public ModVehicleStealthModule1() : base(
            classId: "ModVehicleStealthModule1",
            friendlyName: "Vehicle Stealth Module MK1",
            description: "Presence Masking. Does not stack.")
        {
            string[] stepsToStealthTab = { "MVUM" };
            OnStartedPatching += () => CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "MVSM", "ModVehicle Stealth Modules", StealthModulePatcher.stealthSpriteAtlas, stepsToStealthTab);
        }

        public override EquipmentType EquipmentType => VehicleFramework.VehicleBuilder.ModuleType;

        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;

        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;

        public override string[] StepsToFabricatorTab => new string[] { "MVUM", "MVSM" };
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
                    new Ingredient(TechType.FiberMesh, 1),
                    new Ingredient(TechType.Quartz, 1),
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
