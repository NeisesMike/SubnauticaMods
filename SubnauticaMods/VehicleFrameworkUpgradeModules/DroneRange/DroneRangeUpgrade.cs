using System.Collections.Generic;
using System.Linq;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.VehicleTypes;
using VehicleFramework;

namespace DroneRange
{
    public class DroneRangeUpgrade : ModVehicleUpgrade
    {
        public override string ClassId => "DroneRangeModule";
        public override string DisplayName => "Drone Range Upgrade";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
                {
                    new VehicleFramework.Assets.Ingredient(TechType.AdvancedWiringKit, 1),
                    new VehicleFramework.Assets.Ingredient(TechType.ComputerChip, 1)
                };
        public override string Description => "Boosts the effective operating range of drones by 200 meters. Stacks.";
        public override Atlas.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("DroneRangeIcon.png");
        public override string TabName => "MVCM";
        public override void OnAdded(AddActionParams param)
        {
            Drone drone = param.vehicle as Drone;
            if (drone != null)
            {
                drone.addedConnectionDistance = 200 * drone.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Count();
            }
            else
            {
                VehicleFramework.Logger.Output("This upgrade has no effect on this vehicle.");
            }
        }
        public override void OnRemoved(AddActionParams param)
        {
            Drone drone = param.vehicle as Drone;
            if (drone != null)
            {
                drone.addedConnectionDistance = 200 * drone.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Count();
            }
        }
    }
}