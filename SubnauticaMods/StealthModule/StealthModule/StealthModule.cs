using System;
using UnityEngine;
using VehicleFramework;

namespace StealthModule
{
    public enum StealthQuality
    {
        None,
        Low,
        Medium,
        High,
        Debug
    }

    public class StealthModule : MonoBehaviour
    {
        public StealthQuality quality = StealthQuality.None;
        private static StealthQuality GetQuality(string name)
        {
            if(name.Contains("StealthModule1"))
            {
                return StealthQuality.Low;
            }
            if (name.Contains("StealthModule2"))
            {
                return StealthQuality.Medium;
            }
            if (name.Contains("StealthModule3"))
            {
                return StealthQuality.High;
            }
            return StealthQuality.None;
        }
        public void UpdateQuality()
        {
            StealthQuality result = StealthQuality.None;
            foreach(string upgrade in gameObject.GetComponent<Vehicle>().GetCurrentUpgrades())
            {
                result = (StealthQuality)Math.Max((int)result, (int)GetQuality(upgrade));
            }
            quality = result;
        }
    }
}
