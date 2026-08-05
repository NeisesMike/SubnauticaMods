using System.Collections.Generic;
using System.Linq;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using VehicleFramework.Extensions;

namespace SolarChargingModule
{
    public class SolarChargingModule : ModVehicleUpgrade
    {
        public override string ClassId => "SolarChargingModule";
        public override string DisplayName => Language.main.Get("SolarChargingModule");
        public override string Description => Language.main.Get("Tooltip_SolarChargingModule");
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.AdvancedWiringKit, 2),
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.EnameledGlass, 2)
                };

        public override UnityEngine.Sprite Icon => SpriteHelper.GetSprite("SolarChargingModuleIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            VFSolarCharger solarCharger = param.vehicle.gameObject.EnsureComponent<VFSolarCharger>();
            solarCharger.numChargers = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains("SolarChargingModule")).Count();
            solarCharger.UpdateSetup();
        }
        public override void OnRemoved(AddActionParams param)
        {
            VFSolarCharger solarCharger = param.vehicle.gameObject.EnsureComponent<VFSolarCharger>();
            solarCharger.numChargers = param.vehicle.GetCurrentUpgrades().Where(x => x.Contains("SolarChargingModule")).Count();
            solarCharger.UpdateSetup();
        }
        public override void OnCyclops(AddActionParams param)
        {
            VFSolarCharger solarCharger = param.cyclops.gameObject.EnsureComponent<VFSolarCharger>();
            solarCharger.numChargers = param.cyclops.GetCurrentUpgrades().Where(x => x.Contains("SolarChargingModule")).Count();
            solarCharger.UpdateSetup();
        }
    }
}