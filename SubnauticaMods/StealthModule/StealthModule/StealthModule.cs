using System;
using UnityEngine;
using VehicleFramework.Extensions;

namespace StealthModule
{
    public enum StealthQuality
    {
        None,
        Low,
        Medium,
        High,
        Higher,
        Highest,
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
            if (name.Contains("StealthModule4"))
            {
                return StealthQuality.Higher;
            }
            if (name.Contains("StealthModule5"))
            {
                return StealthQuality.Highest;
            }
            return StealthQuality.None;
        }
        public void UpdateQuality()
        {
            StealthQuality result = StealthQuality.None;
            if (gameObject.GetComponent<Vehicle>() != null)
            {
                foreach (string upgrade in gameObject.GetComponent<Vehicle>().GetCurrentUpgrades())
                {
                    result = (StealthQuality)Math.Max((int)result, (int)GetQuality(upgrade));
                }
            }
            else if(gameObject.GetComponent<SubRoot>() != null)
            {
                foreach (string upgrade in gameObject.GetComponent<SubRoot>().GetCurrentUpgrades())
                {
                    result = (StealthQuality)Math.Max((int)result, (int)GetQuality(upgrade));
                }
            }
            quality = result;
        }
        internal static float GetMaxRange(StealthQuality thisVehicleSQ)
        {
            float myMaxRange;
            switch (thisVehicleSQ)
            {
                case (StealthQuality.None):
                    myMaxRange = float.MaxValue;
                    break;
                case (StealthQuality.Low):
                    myMaxRange = 80f;
                    break;
                case (StealthQuality.Medium):
                    myMaxRange = 60f;
                    break;
                case (StealthQuality.High):
                    myMaxRange = 40f;
                    break;
                case (StealthQuality.Higher):
                    myMaxRange = 20f;
                    break;
                case (StealthQuality.Highest):
                    myMaxRange = 3f;
                    break;
                case (StealthQuality.Debug):
                    myMaxRange = float.MinValue;
                    break;
                default:
                    myMaxRange = float.MaxValue;
                    break;
            }
            return myMaxRange;
        }
    }
}
