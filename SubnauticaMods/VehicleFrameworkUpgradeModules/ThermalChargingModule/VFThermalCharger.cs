using UnityEngine;
using VehicleFramework;
using System.Linq;

namespace ThermalChargingModule
{
    public class VFThermalCharger : MonoBehaviour
    {
		private static readonly float minPower = 0f;
		private static readonly float maxPower = 0.6f;
		private static readonly float minTemp = 30f;
		private static readonly float maxTemp = 90f;
		private int _count = 0;
		public int count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
				EnsureTempDamageCompat(value);
			}
		}
		public float originalMinDamageTemp = 0;
		public void Start()
        {
			originalMinDamageTemp = GetComponent<TemperatureDamage>().minDamageTemperature;
        }
		public void Update()
		{
			if (count > 0)
			{
				ModVehicle mv = GetComponent<ModVehicle>();
				float temperature = mv.GetTemperature();
				float num = EvaluateTemperatureCharge(temperature);
				AddChargeToMV(GetComponent<ModVehicle>(), count * num * Time.deltaTime);
			}
		}
		private float EvaluateTemperatureCharge(float temp)
        {
			if(temp <= minTemp)
            {
				return 0;
            }
			if(temp >= maxTemp)
            {
				return maxPower;
            }
			float tempProgress = (temp - minPower) / (maxPower - minPower);
			return Mathf.Lerp(minPower, maxPower, tempProgress);
        }
		private void AddChargeToMV(ModVehicle mv, float chargeToAdd)
		{
			float chargeRemaining = chargeToAdd;
			foreach (var battery in mv.Batteries)
			{
				EnergyMixin thisBattEM = battery.BatterySlot.gameObject.GetComponent<EnergyMixin>();
				if (thisBattEM is null)
				{
					continue;
				}
				chargeRemaining -= thisBattEM.ModifyCharge(chargeRemaining);
			}
		}
		public void EnsureTempDamageCompat(int numModules)
		{
			if (numModules > 0)
			{
				GetComponent<TemperatureDamage>().minDamageTemperature = maxTemp;
			}
			else
			{
				GetComponent<TemperatureDamage>().minDamageTemperature = originalMinDamageTemp;
			}
		}
	}
}
