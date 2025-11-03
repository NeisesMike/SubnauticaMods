using System.Collections.Generic;
using UnityEngine;
using VehicleFramework.UpgradeTypes;
using VehicleFramework.Assets;

namespace ImpulseSpeedBooster
{
    public class ImpulseSpeedBooster : SelectableChargeableUpgrade
    {
        public override string ClassId => "ImpulseSpeedBooster";
        public override string DisplayName => "Impulse Speed Booster";
        public override string Description => "Charge to unleash a swift propulsion burst, catapulting the sub forward with increased velocity.";
        public override List<Ingredient> Recipe => new List<Ingredient>()
                {
                    new Ingredient(TechType.PowerCell, 2),
                    new Ingredient(TechType.Magnetite, 2),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Titanium, 2)
                };
        public override UnityEngine.Sprite Icon => SpriteHelper.GetSprite("ImpulseSpeedBoosterIcon.png");
        public override void OnSelected(SelectableChargeableActionParams param)
        {
            FMODUWE.PlayOneShot("event:/sub/seamoth/pulse", param.vehicle.transform.position, param.slotCharge);

            string name = param.vehicle.name;
            VehicleFramework.Logger.Log(name);
            float power;
            if (name.ToLower().Contains("seamoth"))
            {
                power = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue("Impulse Power");
            }
            else if (name.ToLower().Contains("exosuit"))
            {
                power = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetPrawnConfig().GetValue("Impulse Power");
            }
            else
            {
                string mvName = param.vehicle.GetComponent<TechTag>().type.AsString();
                power = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue("Impulse Power");
            }
            param.vehicle.useRigidbody.AddForce(power * param.vehicle.transform.forward * param.charge * param.vehicle.useRigidbody.mass * 3, ForceMode.Impulse);
        }
        public override void OnAdded(AddActionParams param)
        {
        }
        public override void OnRemoved(AddActionParams param)
        {
        }
        public override float MaxCharge => 30f;
        public override float EnergyCost => 5f;
    }
}