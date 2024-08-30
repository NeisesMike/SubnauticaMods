using UnityEngine;
using VehicleFramework;

namespace SolarChargingModule
{
    public class VFSolarCharger : MonoBehaviour
	{
		private int m_numChargers = 0;
		public int numChargers
        {
			get
            {
				return m_numChargers;
            }
			set
            {
				m_numChargers = value;
				if(m_numChargers < 0)
                {
					m_numChargers = 0;
                }
            }
        }
		private void UpdateSolarRecharge()
		{
			DayNightCycle main = DayNightCycle.main;
			if (main == null)
			{
				return;
			}
			float num = Mathf.Clamp01((200f + transform.position.y) / 200f);
			float localLightScalar = main.GetLocalLightScalar();
			float amount = 1f * localLightScalar * num * (float)m_numChargers;
			AddChargeToMV(GetComponent<ModVehicle>(), amount);
		}
		public void Start()
        {
			InvokeRepeating(nameof(UpdateSolarRecharge), 1f, 1f);
        }
		private void AddChargeToMV(ModVehicle mv, float chargeToAdd)
        {
			float chargeRemaining = chargeToAdd;
			foreach(var battery in mv.Batteries)
            {
				EnergyMixin thisBattEM = battery.BatterySlot.gameObject.GetComponent<EnergyMixin>();
				if(thisBattEM is null)
                {
					continue;
                }
				chargeRemaining -= thisBattEM.ModifyCharge(chargeRemaining);
			}
		}
	}
}
