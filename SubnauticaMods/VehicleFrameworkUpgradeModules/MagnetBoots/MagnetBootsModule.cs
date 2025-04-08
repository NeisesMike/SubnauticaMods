using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace MagnetBoots
{
    public class MagnetBootsModule : ModVehicleUpgrade
    {
        public override string ClassId => "MagnetBootsModule";
        public override string DisplayName => "Magnet Boots Module";
        public override List<VehicleFramework.Assets.Ingredient> Recipe => new List<VehicleFramework.Assets.Ingredient>()
                {
                    new VehicleFramework.Assets.Ingredient(TechType.Magnetite, 1),
                    new VehicleFramework.Assets.Ingredient(TechType.Nickel, 1),
                    new VehicleFramework.Assets.Ingredient(TechType.PrecursorIonCrystal, 1),
                    new VehicleFramework.Assets.Ingredient(TechType.Polyaniline, 1)
                };
        public override string Description => "Gain the ability to magnetically attach the upgraded vehicle to vehicles and bases on its ventral side (below it).";
        public override Atlas.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("MagnetBootsIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            UpdateMagnetBoots(param.vehicle);
        }
        public override void OnRemoved(AddActionParams param)
        {
            UpdateMagnetBoots(param.vehicle);
        }
        private void UpdateMagnetBoots(Vehicle vehicle)
        {
            var boots = vehicle.gameObject.EnsureComponent<VehicleFramework.VehicleComponents.MagnetBoots>();
            boots.enabled = GetNumberInstalled(vehicle) > 0;
            VehicleFramework.VehicleComponents.MagnetBoots.colliderPairsPerFrame = MainPatcher.MagnetConfig.pairsPerFrame;
        }
    }
}
