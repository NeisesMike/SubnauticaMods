using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace StealthModule
{
    public class StealthUpgradeMk3 : ModVehicleUpgrade
    {
        public override string ClassId => "StealthModule3";
        public override string DisplayName => "Stealth Module Mk 3";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
        {
            new VehicleFramework.Assets.Ingredient(TechType.AramidFibers, 1),
            new VehicleFramework.Assets.Ingredient(TechType.Nickel, 1),
            new VehicleFramework.Assets.Ingredient(TechType.Gold, 1)
        };
        public override string Description => "Creatures beyond 40 meters will not be aggressive or attack.";
        public override Atlas.Sprite Icon => MainPatcher.stealthIcon;
        public override string TabName => MainPatcher.tabName;
        public override string TabDisplayName => MainPatcher.tabDisplayName;
        public override Atlas.Sprite TabIcon => MainPatcher.stealthIcon;
        public override void OnAdded(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<StealthModule>().UpdateQuality();
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<StealthModule>().UpdateQuality();
        }
    }
}
