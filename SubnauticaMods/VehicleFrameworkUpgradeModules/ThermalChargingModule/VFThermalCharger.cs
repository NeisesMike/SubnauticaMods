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
				Vehicle mv = GetComponent<Vehicle>();
				float temperature = mv.GetTemperature();
				float num = EvaluateTemperatureCharge(temperature);
				AddChargeToMV(GetComponent<Vehicle>(), count * num * Time.deltaTime);
			}
		}
		private float EvaluateTemperatureCharge(float temp)
		{
			if (temp <= minTemp)
			{
				return 0;
			}
			if (temp >= maxTemp)
			{
				return maxPower;
			}
			float tempProgress = (temp - minPower) / (maxPower - minPower);
			return Mathf.Lerp(minPower, maxPower, tempProgress);
		}
		private void AddChargeToMV(Vehicle mv, float chargeToAdd)
		{
			float chargeRemaining = chargeToAdd;
			foreach (var battery in mv.energyInterface.sources)
			{
				if (battery is null)
				{
					continue;
				}
				chargeRemaining -= battery.ModifyCharge(chargeRemaining);
				if (chargeRemaining < 0)
				{
					break;
				}
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
