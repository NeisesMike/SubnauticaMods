using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VehicleFramework;

namespace VFSpeed
{
    internal enum VehicleType
    {
        vanilla,
        mod,
        cyclops
    }
    internal class VFSpeedModule : MonoBehaviour
    {
        private VehicleType vt = VehicleType.vanilla;
        private Vehicle vehicle = null;
        private ModVehicle mv= null;
        private SubRoot cyclops = null;
        VehicleAccelerationModifier vam = null;
        private void Start()
        {
            if (gameObject.name.ToLower().Contains("cyclops"))
            {
                cyclops = gameObject.GetComponent<SubRoot>();
                vt = VehicleType.cyclops;
                vam = cyclops.gameObject.AddComponent<VehicleAccelerationModifier>();
                vam.accelerationMultiplier = 1;
                cyclops.GetComponent<SubControl>().accelerationModifiers = cyclops.GetComponent<SubControl>().accelerationModifiers.Append(vam).ToArray(); ;
            }
            else if(gameObject.GetComponent<ModVehicle>() != null)
            {
                mv = gameObject.GetComponent<ModVehicle>();
                vt = VehicleType.mod;
                vam = mv.gameObject.AddComponent<VehicleAccelerationModifier>();
                vam.accelerationMultiplier = 1;
            }
            else if (gameObject.GetComponent<Vehicle>() != null)
            {
                vehicle = gameObject.GetComponent<Vehicle>();
                vt = VehicleType.vanilla;
                vam = vehicle.gameObject.AddComponent<VehicleAccelerationModifier>();
                vam.accelerationMultiplier = 1;
                vehicle.accelerationModifiers = vehicle.accelerationModifiers.Append(vam).ToArray();
            }
            else
            {
                DestroyImmediate(this);
            }
        }
        private void FixedUpdate()
        {
            float strength = GetUpgradeTotal();
            if(strength < 1)
            {
                return;
            }
            strength *= MainPatcher.config.intensity;
            switch (vt)
            {
                case VehicleType.vanilla:
                    SpendVanillaEnergy(strength);
                    break;
                case VehicleType.mod:
                    SpendModEnergy(strength);
                    break;
                case VehicleType.cyclops:
                    SpendCyclopsEnergy(strength);
                    break;
            }
            vam.accelerationMultiplier = Mathf.Pow(1.1f, strength);
        }
        private void SpendVanillaEnergy(float strength)
        {
            float cost = GetPowerExpenditure(strength);
            vehicle.GetComponent<EnergyInterface>().ConsumeEnergy(cost * Time.fixedDeltaTime);
        }
        private void SpendModEnergy(float strength)
        {
            float cost = GetPowerExpenditure(strength);
            mv.powerMan.TrySpendEnergy(cost * Time.fixedDeltaTime);
        }
        private void SpendCyclopsEnergy(float strength)
        {
            float cost = GetPowerExpenditure(strength);
            cyclops.powerRelay.ConsumeEnergy(cost * Time.fixedDeltaTime, out _);
        }
        private float GetPowerExpenditure(float strength)
        {
            Vector3 moveDirection = GameInput.GetMoveDirection();
            float scalarFactor = 0.03f;
            float basePowerConsumptionPerSecond = moveDirection.x + moveDirection.y + moveDirection.z;
            float upgradeModifier = Mathf.Pow(1.1f, strength);
            return scalarFactor * basePowerConsumptionPerSecond * upgradeModifier;
        }
        private int GetUpgradeTotal()
        {
            List<string> upgrades = null;
            switch (vt)
            {
                case VehicleType.vanilla:
                    upgrades = vehicle.GetCurrentUpgrades();
                    break;
                case VehicleType.mod:
                    upgrades = mv.GetCurrentUpgrades();
                    break;
                case VehicleType.cyclops:
                    upgrades = cyclops.GetCurrentUpgrades();
                    break;
            }
            if(upgrades == null)
            {
                return 0;
            }
            return upgrades.Select(x => UpgradeNameToPotency(x)).Sum();
        }
        private int UpgradeNameToPotency(string name)
        {
            if (name.Contains(Names.speedName))
            {
                string number = name.Substring(Names.speedName.Length, 1);
                return int.Parse(number);
            }
            return 0;
        }
    }
}
