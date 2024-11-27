using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using VehicleFramework.Localization;

namespace VoidDepth
{
    public class VoidDepth : ModVehicleUpgrade
    {
        public const string upgradeName = "VoidDepthUpgrade";
        public override string ClassId => upgradeName;
        public override string DisplayName => Localizer<EnglishString>.GetString(EnglishString.ModuleDisplayName);
        public override string Description => Localizer<EnglishString>.GetString(EnglishString.ModuleDescription);
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
        public override void OnCyclops(AddActionParams param)
        {
            param.cyclops.gameObject.GetComponent<CrushDamage>().UpdateDepthClassification();
        }
    }
}