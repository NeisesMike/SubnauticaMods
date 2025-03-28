using System.Collections.Generic;
using VehicleFramework;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using System.Linq;

namespace FlightModule
{
    public class FlightModule : ModVehicleUpgrade
    {
        private Dictionary<TechType, bool> moveAboveWaterDefault = new Dictionary<TechType, bool>();
        private Dictionary<TechType, bool> rotateAboveWaterDefault = new Dictionary<TechType, bool>();
        private Dictionary<TechType, float> dragAboveWaterDefault = new Dictionary<TechType, float>();
        private Dictionary<TechType, float> gravityAboveWaterDefault = new Dictionary<TechType, float>();
        public override string ClassId => "FlightModule";
        public override string DisplayName => "Flight Module";
        public override string Description => "Allows the upgraded vehicle to take flight";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Aerogel, 2),
                };

        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("FlightModuleIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            UpdateFlyingModule(param.vehicle);
        }
        public override void OnRemoved(AddActionParams param)
        {
            UpdateFlyingModule(param.vehicle);
        }
        public override void OnCyclops(AddActionParams param)
        {
            UpdateFlyingModule(param.cyclops);
        }
        private void UpdateFlyingModule(Vehicle vehicle)
        {
            if (GetNumberInstalled(vehicle) > 0)
            {
                if (vehicle is ModVehicle mv)
                {
                    EnableModFlight(mv);
                }
                else if (vehicle is SeaMoth seamoth)
                {
                    EnableSeamothFlight(seamoth);
                }
            }
            else
            {
                if (vehicle is ModVehicle mv)
                {
                    DisableModFlight(mv);
                }
                else if (vehicle is SeaMoth seamoth)
                {
                    DisableSeamothFlight(seamoth);
                }
            }
        }
        private void UpdateFlyingModule(SubRoot cyclops)
        {
            int numUpgrades = cyclops.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Count();
            if (numUpgrades > 0)
            {
                EnableCyclopsFlight(cyclops);
            }
            else
            {
                DisableCyclopsFlight(cyclops);
            }
        }
        private void DisableSeamothFlight(SeaMoth seamoth)
        {
            seamoth.worldForces.aboveWaterGravity = 9.81f;
            seamoth.worldForces.aboveWaterDrag = 1f;
            seamoth.moveOnLand = false;
        }
        private void EnableSeamothFlight(SeaMoth seamoth)
        {
            seamoth.worldForces.aboveWaterGravity = 4;
            seamoth.worldForces.aboveWaterDrag = 0.8f;
            seamoth.moveOnLand = true;
        }
        private void DisableModFlight(ModVehicle mv)
        {
            var engine = mv.Engine ?? (mv.VFEngine as VehicleFramework.Engines.ModVehicleEngine);
            if (engine != null)
            {
                engine.CanRotateAboveWater = rotateAboveWaterDefault[mv.TechType];
                engine.CanMoveAboveWater = moveAboveWaterDefault[mv.TechType];
                mv.worldForces.aboveWaterGravity = gravityAboveWaterDefault[mv.TechType];
                mv.worldForces.aboveWaterDrag = dragAboveWaterDefault[mv.TechType];
            }
        }
        private void EnableModFlight(ModVehicle mv)
        {
            var engine = mv.Engine ?? (mv.VFEngine as VehicleFramework.Engines.ModVehicleEngine);
            if (engine != null)
            {
                RegisterDefaults(mv);
                engine.CanRotateAboveWater = true;
                engine.CanMoveAboveWater = true;
                mv.worldForces.aboveWaterGravity = 2f;
                mv.worldForces.aboveWaterDrag = 0.8f;
            }
        }
        private void RegisterDefaults(ModVehicle mv)
        {
            var engine = mv.Engine ?? (mv.VFEngine as VehicleFramework.Engines.ModVehicleEngine);
            if (!moveAboveWaterDefault.ContainsKey(mv.TechType))
            {
                moveAboveWaterDefault.Add(mv.TechType, engine.CanMoveAboveWater);
            }
            if (!rotateAboveWaterDefault.ContainsKey(mv.TechType))
            {
                rotateAboveWaterDefault.Add(mv.TechType, engine.CanRotateAboveWater);
            }
            if (!dragAboveWaterDefault.ContainsKey(mv.TechType))
            {
                dragAboveWaterDefault.Add(mv.TechType, mv.worldForces.aboveWaterDrag);
            }
            if (!gravityAboveWaterDefault.ContainsKey(mv.TechType))
            {
                gravityAboveWaterDefault.Add(mv.TechType, mv.worldForces.aboveWaterGravity);
            }
        }
        private void DisableCyclopsFlight(SubRoot cyclops)
        {
            cyclops.worldForces.aboveWaterDrag = 1f;
            cyclops.worldForces.aboveWaterGravity = 9.81f;
        }
        private void EnableCyclopsFlight(SubRoot cyclops)
        {
            cyclops.worldForces.aboveWaterDrag = 0.6f;
            cyclops.worldForces.aboveWaterGravity = 3f;
        }
    }
}