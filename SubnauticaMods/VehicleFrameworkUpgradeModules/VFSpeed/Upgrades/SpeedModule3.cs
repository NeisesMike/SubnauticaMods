using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace VFSpeed
{
    public class SpeedModuleMk3 : ModVehicleUpgrade
    {
        public const string upgradeName = Names.speedName + "3";
        public override string ClassId => upgradeName;
        public override string DisplayName => Names.speedDisplayName + " 3";
        public override string Description => "Greatly increases vehicle acceleration and top speed at a dramatic cost of power.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
        {
            new Ingredient(TechType.PrecursorIonPowerCell, 1),
            new Ingredient(TechType.Kyanite, 2),
            new Ingredient(TechType.Sulphur, 1)
        };
        public override UnityEngine.Sprite Icon => SpriteHelper.GetSprite("SpeedModuleIcon3.png");
        public override string TabName => Names.tabName;
        public override string TabDisplayName => Names.tabDisplayName;
        public override UnityEngine.Sprite TabIcon => SpriteHelper.GetSprite("SpeedModuleIcon3.png");
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