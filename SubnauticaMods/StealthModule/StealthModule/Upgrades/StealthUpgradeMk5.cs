using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace StealthModule
{
    public class StealthUpgradeMk5 : ModVehicleUpgrade
    {
        public override string ClassId => "StealthModule5";
        public override string DisplayName => "Stealth Module Mk 5";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
        {
            new VehicleFramework.Assets.Ingredient(TechType.PrecursorIonCrystal, 1),
            new VehicleFramework.Assets.Ingredient(TechType.RedGreenTentacleSeed, 1),
            new VehicleFramework.Assets.Ingredient(TechType.Polyaniline, 1)
        };
        public override string Description => "Creatures beyond 3 meters will not be aggressive or attack.";
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
        public override void OnCyclops(AddActionParams param)
        {
            param.cyclops.gameObject.EnsureComponent<StealthModule>().UpdateQuality();
        }
    }
}
