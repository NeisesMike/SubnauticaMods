using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace CrabsquidModule
{
    public class CrabsquidModule : ModVehicleUpgrade
    {
        public override string ClassId => "CrabsquidModule";
        public override string DisplayName => "Crabsquid Protection Module";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 2),
                    new Ingredient(TechType.Titanium, 1)
                };
        public override string Description => "Equip to shrug off Crabsquid EMP at a small cost of energy.";
        public override UnityEngine.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("CrabsquidModuleIcon.png");
        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }
    }
}
