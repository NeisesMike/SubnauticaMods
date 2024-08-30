using System.Collections.Generic;
using System.Linq;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace SolarChargingModule
{
    public class SolarChargingModule : ModVehicleUpgrade
    {
        public override string ClassId => "SolarChargingModule";
        public override string DisplayName => "Solar Charging Module";
        public override string Description => "Recharge your ship's batteries with solar energy during daylight; effectiveness increases with sun elevation and proximity to the surface.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.AdvancedWiringKit, 2),
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.EnameledGlass, 2)
                };

        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("SolarChargingModuleIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            param.mv.gameObject.EnsureComponent<VFSolarCharger>().numChargers = param.mv.GetCurrentUpgrades().Where(x => x.Contains("SolarChargingModule")).Count();
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.mv.gameObject.EnsureComponent<VFSolarCharger>().numChargers = param.mv.GetCurrentUpgrades().Where(x => x.Contains("SolarChargingModule")).Count();
        }
    }
}