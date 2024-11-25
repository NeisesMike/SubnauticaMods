using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace VoidDepth
{
    public class VoidDepth : ModVehicleUpgrade
    {
        public const string upgradeName = "VoidDepthUpgrade";
        public const float crushAddition = 5000;
        public override string ClassId => upgradeName;
        public override string DisplayName => "Void Depth Module";
        public override string Description => "Combines advanced alloys with the enigmatic properties of Ghost Weed to achieve extraordinary crush depth capabilities. Stacks.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
        {
            new Ingredient(TechType.Kyanite, 2),
            new Ingredient(TechType.PlasteelIngot, 1),
            new Ingredient(TechType.Nickel, 1),
            new Ingredient(TechType.RedGreenTentacleSeed, 1),
        };
        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("VoidDepthIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            param.vehicle.crushDamage.UpdateDepthClassification();
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.vehicle.crushDamage.UpdateDepthClassification();
        }
    }
}