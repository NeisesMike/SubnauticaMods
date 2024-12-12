using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace VFSpeed
{
    public class SpeedModuleMk2 : ModVehicleUpgrade
    {
        public const string upgradeName = Names.speedName + "2";
        public override string ClassId => upgradeName;
        public override string DisplayName => Names.speedDisplayName + " 2";
        public override string Description => "Significantly increases vehicle acceleration and top speed at a great cost of power.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
        {
            new Ingredient(TechType.PowerCell, 1),
            new Ingredient(TechType.Lithium, 2),
            new Ingredient(TechType.Aerogel, 1)
        };
        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("SpeedModuleIcon2.png");
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