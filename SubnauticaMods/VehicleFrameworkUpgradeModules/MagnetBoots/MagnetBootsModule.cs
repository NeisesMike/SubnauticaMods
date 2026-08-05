using System.Collections.Generic;
using VehicleFramework.UpgradeTypes;

namespace MagnetBoots
{
    public class MagnetBootsModule : ModVehicleUpgrade
    {
        public override string ClassId => "MagnetBootsModule";
        public override string DisplayName => Language.main.Get("MagnetBootsModule");
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Nickel, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Polyaniline, 1)
                };
        public override string Description => Language.main.Get("Tooltip_MagnetBootsModule");
        public override UnityEngine.Sprite Icon => VehicleFramework.Assets.SpriteHelper.GetSprite("MagnetBootsIcon.png");
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
            var boots = vehicle.gameObject.EnsureComponent<VehicleFramework.VehicleRootComponents.MagnetBoots>();
            boots.enabled = GetNumberInstalled(vehicle) > 0;
            VehicleFramework.VehicleRootComponents.MagnetBoots.ColliderPairsPerFrame = MainPatcher.MagnetConfig.pairsPerFrame;
        }
    }
}
