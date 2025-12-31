using UnityEngine;

namespace SelfRepairModule
{
    internal class SelfRepairBehavior : SelfRepairRoot
    {
        private Vehicle Vehicle = null;
        internal override float GetRepairAmount()
        {
            if (Vehicle is VehicleFramework.ModVehicle)
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
        internal override float GetRepairEnergyCost()
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
        internal override void Awake()
        {
            base.Awake();
            Vehicle = gameObject.GetComponent<Vehicle>();
            if (Vehicle == null)
            {
                Component.DestroyImmediate(this);
            }
        }
        internal override bool IsReady()
        {
            bool isWounded = Vehicle.liveMixin.GetHealthFraction() < 1;
            bool hasPower = Vehicle.GetComponent<EnergyInterface>().hasCharge || !GameModeUtils.RequiresPower();
            bool isEnabled = true;
            if(Vehicle is VehicleFramework.ModVehicle mv)
            {
                isEnabled &= mv.IsPoweredOn;
            }
            return isWounded && hasPower && isEnabled;
        }
        internal override void SpendEnergy(float energyCost)
        {
            Vehicle.GetComponent<EnergyInterface>().ConsumeEnergy(energyCost);
        }
        internal override void DoRepair(float repairAmount)
        {
            Vehicle.liveMixin.AddHealth(repairAmount);
        }
        internal override float GetAffordableRepair(float potentialRepair)
        {
            Vehicle.GetEnergyValues(out float charge, out float _);
            float repairBudget = ConvertPowerToRepair(charge);
            return Mathf.Min(repairBudget, potentialRepair);
        }
    }
}
