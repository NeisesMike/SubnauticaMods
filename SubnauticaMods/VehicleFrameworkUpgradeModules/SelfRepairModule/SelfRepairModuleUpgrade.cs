using System.Collections.Generic;
using UnityEngine;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Extensions;
using System.Linq;

namespace SelfRepairModule
{
    public class SelfRepairModuleUpgrade : ModVehicleUpgrade
    {
        public override string ClassId => "SelfRepairModule";
        public override string DisplayName => "Self-Repair Module";
        public override string Description => "This upgrade allows a vehicle to consume power in order to repair itself.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.Welder, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.Lithium, 1)
                };
        public override Sprite Icon => SpriteManager.Get(TechType.FirstAidKit);
        public override void OnAdded(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<SelfRepairBehavior>().enabled = true;
        }
        public override void OnRemoved(AddActionParams param)
        {
            param.vehicle.gameObject.EnsureComponent<SelfRepairBehavior>().enabled = 0 < GetNumberInstalled(param.vehicle);
        }
        public override void OnCyclops(AddActionParams param)
        {
            param.cyclops.gameObject.EnsureComponent<CyclopsSelfRepairBehavior>().enabled = param.cyclops.GetCurrentUpgrades().Where(x => x.Contains(ClassId)).Any();
        }
    }
}