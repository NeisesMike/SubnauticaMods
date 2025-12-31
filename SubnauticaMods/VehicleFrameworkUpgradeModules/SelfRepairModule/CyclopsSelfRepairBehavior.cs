using UnityEngine;
using System.Linq;

namespace SelfRepairModule
{
    // num damage points is 10.
    // cyclops health is 1500
    // that's 150 hp per damage point
    // but a damage point only truly has 35 health
    internal class CyclopsSelfRepairBehavior : SelfRepairRoot
    {
        private SubRoot Cyclops = null;
        private CyclopsExternalDamageManager cedm = null;
        internal override float GetRepairAmount()
        {
            return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(Configuration.amountOptionName);
        }
        internal override float GetRepairEnergyCost()
        {
            return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(Configuration.costOptionName);
        }
        internal override void Awake()
        {
            base.Awake();
            Cyclops = gameObject.GetComponent<SubRoot>();
            if (Cyclops == null)
            {
                Component.DestroyImmediate(this);
            }
            //InvokeRepeating(nameof(ActivateRepair), 0f, 150f / GetRepairAmount());
        }
        internal override bool IsReady()
        {
            bool isWounded = Cyclops.GetComponent<LiveMixin>().GetHealthFraction() < 1;
            bool hasPower = Cyclops.powerRelay.IsPowered();
            return isWounded && hasPower;
        }
        internal override void DoRepair(float repairAmount)
        {
            if (cedm == null)
            {
                cedm = Cyclops.GetComponentInChildren<CyclopsExternalDamageManager>();
            }
            // do a little repair on the healthiest damage point
            cedm.damagePoints
                .Where(x => !cedm.unusedDamagePoints.Contains(x))
                .Select(x => x.liveMixin)
                .OrderBy(x => x.health)
                .LastOrDefault()
                ?.AddHealth(repairAmount);
        }
        internal override void SpendEnergy(float realizedRepair)
        {
            float realizedCost = ConvertRepairToPower(realizedRepair);
            Cyclops.powerRelay.ConsumeEnergy(realizedCost, out float _);
        }
        internal override float GetAffordableRepair(float potentialRepair)
        {
            float availableCharge = Cyclops.powerRelay.GetPower();
            float repairBudget = ConvertPowerToRepair(availableCharge);
            return Mathf.Min(repairBudget, potentialRepair);
        }
    }
}
