using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace StealthModule
{
    public class StealthUpgradeMk1 : ModVehicleUpgrade
    {
        public override string ClassId => "StealthModule1";
        public override string DisplayName => "Stealth Module Mk 1";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
        {
            new VehicleFramework.Assets.Ingredient(TechType.FiberMesh, 1),
            new VehicleFramework.Assets.Ingredient(TechType.Quartz, 1),
            new VehicleFramework.Assets.Ingredient(TechType.Gold, 1)
        };
        public override string Description => "Creatures beyond 80 meters will not be aggressive or attack.";
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
