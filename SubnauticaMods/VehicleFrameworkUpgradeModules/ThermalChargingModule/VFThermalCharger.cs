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
		public int Count
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
			TemperatureDamage td = GetComponent<TemperatureDamage>();
			if (td != null)
			{
				originalMinDamageTemp = GetComponent<TemperatureDamage>().minDamageTemperature;
			}
		}
		public void Update()
		{
			if (Count > 0)
			{
				Vehicle vehicle = GetComponent<Vehicle>();
				SubRoot subroot = GetComponent<SubRoot>();
				if (vehicle != null)
				{
					float temperature = vehicle.GetTemperature();
					float num = EvaluateTemperatureCharge(temperature);
					AddChargeToMV(GetComponent<Vehicle>(), Count * num * Time.deltaTime);
				}
				else if (subroot != null)
				{
					float temperature = subroot.GetTemperature();
					float num = EvaluateTemperatureCharge(temperature);
					AddChargeToCyclops(GetComponent<SubRoot>(), Count * num * Time.deltaTime);
				}
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
		private void AddChargeToCyclops(SubRoot subroot, float chargeToAdd)
		{
			subroot.powerRelay.AddEnergy(chargeToAdd, out _);
		}
		public void EnsureTempDamageCompat(int numModules)
		{
			TemperatureDamage td = GetComponent<TemperatureDamage>();
			if (td != null)
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
}
