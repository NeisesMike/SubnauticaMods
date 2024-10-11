using System.Collections.Generic;
using System.Linq;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using VehicleFramework;

namespace ThermalChargingModule
{
    public class ThermalChargingModule : ModVehicleUpgrade
    {
        public override string ClassId => "ThermalChargingModule";
        public override string DisplayName => "Thermal Charging Module";
        public override string Description => "Recharge your ship's batteries with thermal energy in hot locations; effectiveness increases with temperature. Stacks.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.Kyanite, 3)
                };

        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("ThermalChargingModuleIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<VFThermalCharger>().count = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Count();
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<VFThermalCharger>().count = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Count();
        }
    }
}