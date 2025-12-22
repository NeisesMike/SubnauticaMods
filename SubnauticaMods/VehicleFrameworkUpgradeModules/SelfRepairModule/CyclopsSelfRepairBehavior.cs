using System;
using UnityEngine;
using System.Linq;

namespace SelfRepairModuleUpgrade
{
    // num damage points is 10.
    // cyclops health is 1500
    // that's 150 hp per damage point
    internal class CyclopsSelfRepairBehavior : MonoBehaviour
    {
        private SubRoot Cyclops = null;
        private CyclopsDamagePoint currentDamagePoint = null;
        private float RepairAmount => VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(Configuration.amountOptionName);
        private float RepairEnergyCost => VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(Configuration.costOptionName);
        private void Awake()
        {
            Cyclops = gameObject.GetComponent<SubRoot>();
            if (Cyclops == null)
            {
                Component.DestroyImmediate(this);
            }
            InvokeRepeating(nameof(ActivateRepair), 0f, 150f / RepairAmount);
        }
        private void Update()
        {
            if (currentDamagePoint != null)
            {
                if (!SpendEnergy(RepairAmount * Time.deltaTime))
                {
                    currentDamagePoint = null;
                }
            }
        }
        private bool IsReady()
        {
            bool isWounded = Cyclops.GetComponent<LiveMixin>().GetHealthFraction() < 1;
            bool hasPower = Cyclops.powerRelay.IsPowered();
            return isWounded && hasPower;
        }
        private float ConvertPowerToRepair(float power)
        {
            return Mathf.Min(RepairAmount, RepairAmount * (power / RepairEnergyCost));
        }
        private float ConvertRepairToPower(float repairValue)
        {
            return Mathf.Min(RepairEnergyCost, RepairEnergyCost * (repairValue / RepairAmount));
        }
        private void ActivateRepair()
        {
            if (!IsReady())
            {
                return;
            }
            var cedm = Cyclops.GetComponentInChildren<CyclopsExternalDamageManager>();
            if (cedm == null)
            {
                return;
            }
            // if there is no damage point, return
            if(cedm.unusedDamagePoints.Count == cedm.damagePoints.Count())
            {
                return;
            }
            // if there is a damage point, begin repairing it
            if(currentDamagePoint == null)
            {
                currentDamagePoint = cedm.damagePoints.Where(x => !cedm.unusedDamagePoints.Contains(x)).FirstOrDefault();
            }
            else
            {
                currentDamagePoint.OnRepair();
                currentDamagePoint = null;
                ActivateRepair();
            }
        }

        private bool SpendEnergy(float realizedRepair)
        {
            float realizedCost = ConvertRepairToPower(realizedRepair);
            return Cyclops.powerRelay.ConsumeEnergy(realizedCost, out float _);
        }
        private float GetPotentialRepair()
        {
            float missingHealth = Cyclops.GetComponent<LiveMixin>().maxHealth - Cyclops.GetComponent<LiveMixin>().health;
            return Mathf.Min(missingHealth, RepairAmount);
        }
        private float GetAffordableRepair(float potentialRepair)
        {
            float availableCharge = Cyclops.powerRelay.GetPower();
            float repairBudget = ConvertPowerToRepair(availableCharge);
            return Mathf.Min(repairBudget, potentialRepair);
        }
    }
}
