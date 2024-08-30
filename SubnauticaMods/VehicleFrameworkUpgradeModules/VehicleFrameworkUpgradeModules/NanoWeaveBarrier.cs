using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace NanoWeaveBarrier
{
    public class NanoWeaveBarrier : ModVehicleUpgrade
    {
        public override string ClassId => "NanoWeaveBarrier";
        public override string DisplayName => "Nano-Weave Barrier";
        public override string Description => "Employs nanotechnology to weave an ultra-durable mesh, reinforcing the sub's exterior against environmental hazards.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.AramidFibers, 2),
                    new Ingredient(TechType.Diamond, 1),
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.EnameledGlass, 1)
                };

        public override Atlas.Sprite Icon => SpriteHelper.GetSprite("NanoWeaveBarrierIcon.png");
        public override void OnAdded(AddActionParams param)
        {
            var damg = param.mv.gameObject.EnsureComponent<DamageModifier>();
            damg.damageType = DamageType.Normal;
            damg.multiplier = Mathf.Pow(0.90f, param.mv.GetCurrentUpgrades().Where(x => x.Contains("NanoWeaveBarrier")).Count());
        }
        public override void OnRemoved(AddActionParams param)
        {
            var damg = param.mv.gameObject.EnsureComponent<DamageModifier>();
            damg.damageType = DamageType.Normal;
            damg.multiplier = Mathf.Pow(0.90f, param.mv.GetCurrentUpgrades().Where(x => x.Contains("NanoWeaveBarrier")).Count());
        }

    }
}