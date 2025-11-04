using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace StealthModule
{
    public class StealthUpgradeMk2 : ModVehicleUpgrade
    {
        public override string ClassId => "StealthModule2";
        public override string DisplayName => "Stealth Module Mk 2";
        public override List<Ingredient> Recipe => new List<Ingredient>()
        {
            new Ingredient(TechType.Silicone, 1),
            new Ingredient(TechType.Lithium, 1),
            new Ingredient(TechType.Gold, 1)
        };
        public override string Description => "Creatures beyond 60 meters will not be aggressive or attack.";
        public override UnityEngine.Sprite Icon => MainPatcher.stealthIcon;
        public override string TabName => MainPatcher.tabName;
        public override string TabDisplayName => MainPatcher.tabDisplayName;
        public override UnityEngine.Sprite TabIcon => MainPatcher.stealthIcon;
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
