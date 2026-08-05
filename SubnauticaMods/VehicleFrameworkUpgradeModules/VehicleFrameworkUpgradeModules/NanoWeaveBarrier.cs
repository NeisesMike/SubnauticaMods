using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;
using VehicleFramework.Extensions;

namespace NanoWeaveBarrier
{
    public class NanoWeaveBarrier : ModVehicleUpgrade
    {
        public override string ClassId => "NanoWeaveBarrier";
        public override string DisplayName => Language.main.Get("NanoWeaveBarrier");
        public override string Description => Language.main.Get("Tooltip_NanoWeaveBarrier");
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.AramidFibers, 2),
                    new Ingredient(TechType.Diamond, 1),
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.EnameledGlass, 1)
                };

        public override UnityEngine.Sprite Icon => SpriteHelper.GetSprite("NanoWeaveBarrierIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            var damg = param.vehicle.gameObject.EnsureComponent<DamageModifier>();
            damg.damageType = DamageType.Normal;
            damg.multiplier = Mathf.Pow(0.90f, param.vehicle.GetCurrentUpgrades().Where(x => x.Contains("NanoWeaveBarrier")).Count());
        }
        public override void OnRemoved(AddActionParams param)
        {
            var damg = param.vehicle.gameObject.EnsureComponent<DamageModifier>();
            damg.damageType = DamageType.Normal;
            damg.multiplier = Mathf.Pow(0.90f, param.vehicle.GetCurrentUpgrades().Where(x => x.Contains("NanoWeaveBarrier")).Count());
        }
        public override void OnCyclops(AddActionParams param)
        {
            var damg = param.cyclops.gameObject.EnsureComponent<DamageModifier>();
            damg.damageType = DamageType.Normal;
            damg.multiplier = Mathf.Pow(0.90f, param.cyclops.GetCurrentUpgrades().Where(x => x.Contains("NanoWeaveBarrier")).Count());
        }

    }
}