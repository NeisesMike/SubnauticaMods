using System.Collections.Generic;
using System.Linq;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using VehicleFramework;

namespace ThermalChargingModule
{
    public class ThermalChargingModule : ModVehicleUpgrade
    {
        public const string TCMClassID = "ThermalChargingModule";
        public override string ClassId => TCMClassID;
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
            var vtc = param.vehicle.gameObject.EnsureComponent<VFThermalCharger>();
            vtc.Count = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains(ThermalChargingModule.TCMClassID)).Count();
        }
        public override void OnRemoved(AddActionParams param)
        {
            var vtc = param.vehicle.gameObject.EnsureComponent<VFThermalCharger>();
            vtc.Count = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains(ThermalChargingModule.TCMClassID)).Count();
        }
        public override void OnCyclops(AddActionParams param)
        {
            var vtc = param.cyclops.gameObject.EnsureComponent<VFThermalCharger>();
            vtc.Count = param.cyclops.GetCurrentUpgrades().Where(x => x.Contains(ThermalChargingModule.TCMClassID)).Count();
        }
    }
}