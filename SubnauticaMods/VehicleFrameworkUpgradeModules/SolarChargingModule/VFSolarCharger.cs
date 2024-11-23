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
				if (m_numChargers < 0)
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
			float amount =
				MainPatcher.MyConfig.GetPower()
				* localLightScalar 
				* num 
				* (float)m_numChargers;
			AddChargeToMV(GetComponent<Vehicle>(), amount);
		}
		public void UpdateSetup()
        {
			CancelInvoke();
			InvokeRepeating(nameof(UpdateSolarRecharge), 1f, MainPatcher.MyConfig.GetRepeatRate());
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
	}
}
