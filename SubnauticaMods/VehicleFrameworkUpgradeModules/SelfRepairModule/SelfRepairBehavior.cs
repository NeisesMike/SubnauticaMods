using UnityEngine;

namespace SelfRepairModule
{
    internal class SelfRepairBehavior : MonoBehaviour
    {
        private Vehicle Vehicle = null;
        private float RepairAmount
        {
            get
            {
                if(Vehicle is VehicleFramework.ModVehicle)
                {
                    string mvName = Vehicle.GetComponent<TechTag>().type.AsString();
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(Configuration.amountOptionName);
                }
                else if (Vehicle is SeaMoth)
                {
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(Configuration.amountOptionName);
                }
                else if (Vehicle is Exosuit)
                {
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetPrawnConfig().GetValue(Configuration.amountOptionName);
                }
                throw new System.Exception("Self Repair module added to an unknown Vehicle!");
            }
        }
        private float RepairEnergyCost
        {
            get
            {
                if (Vehicle is VehicleFramework.ModVehicle)
                {
                    string mvName = Vehicle.GetComponent<TechTag>().type.AsString();
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(Configuration.costOptionName);
                }
                else if (Vehicle is SeaMoth)
                {
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(Configuration.costOptionName);
                }
                else if (Vehicle is Exosuit)
                {
                    return VehicleFramework.Admin.ExternalVehicleConfig<float>.GetPrawnConfig().GetValue(Configuration.costOptionName);
                }
                throw new System.Exception("Self Repair module added to an unknown Vehicle!");
            }
        }
        private void Awake()
        {
            Vehicle = gameObject.GetComponent<Vehicle>();
            if (Vehicle == null)
            {
                Component.DestroyImmediate(this);
            }
        }
        private void Update()
        {
            if (IsReady())
            {
                ActivateRepair();
            }
        }
        private bool IsReady()
        {
            bool isWounded = Vehicle.liveMixin.GetHealthFraction() < 1;
            bool hasPower = Vehicle.GetComponent<EnergyInterface>().hasCharge;
            bool isEnabled = true;
            if(Vehicle is VehicleFramework.ModVehicle mv)
            {
                isEnabled &= mv.IsPoweredOn;
            }
            return isWounded && hasPower && isEnabled;
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
            float potentialRepair = GetPotentialRepair() * Time.deltaTime;
            float intendedRepair = GetAffordableRepair(potentialRepair);
            float realizedRepair = Vehicle.liveMixin.AddHealth(intendedRepair);
            SpendEnergy(realizedRepair);
        }
        private void SpendEnergy(float realizedRepair)
        {
            float realizedCost = ConvertRepairToPower(realizedRepair);
            Vehicle.GetComponent<EnergyInterface>().ConsumeEnergy(realizedCost);
        }
        private float GetPotentialRepair()
        {
            float missingHealth = Vehicle.liveMixin.maxHealth - Vehicle.liveMixin.health;
            return Mathf.Min(missingHealth, RepairAmount);
        }
        private float GetAffordableRepair(float potentialRepair)
        {
            Vehicle.GetEnergyValues(out float charge, out float capacity);
            float repairBudget = ConvertPowerToRepair(charge);
            return Mathf.Min(repairBudget, potentialRepair);
        }
    }
}
