using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace VFSpeed
{
    public class SpeedModuleMk1 : ModVehicleUpgrade
    {
        public const string upgradeName = Names.speedName + "1";
        public override string ClassId => upgradeName;
        public override string DisplayName => Names.speedDisplayName + " 1";
        public override string Description => "Slightly increases vehicle acceleration and top speed at a notable cost of power.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
        {
            new Ingredient(TechType.Battery, 1),
            new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.Silver, 1)
        };
        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("SpeedModuleIcon1.png");
        public override string TabName => Names.tabName;
        public override string TabDisplayName => Names.tabDisplayName;
        public override Atlas.Sprite TabIcon => SpriteHelper.GetSprite("SpeedModuleIcon3.png");
        public override void OnAdded(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<VFSpeedModule>();
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<VFSpeedModule>();
        }
        public override void OnCyclops(AddActionParams param)
        {
            param.cyclops.gameObject.EnsureComponent<VFSpeedModule>();
        }
    }
}