using System;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using VehicleFramework;

namespace FlightModule
{
    [HarmonyPatch(typeof(Ocean))]
    public class OceanPatcher
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return typeof(Ocean).GetMethod(nameof(Ocean.GetDepthOf), new Type[] { typeof(GameObject) });
        }

        [HarmonyPostfix]
        public static void OceanGetDepthOfPostfix(GameObject obj, ref float __result)
        {
            SubRoot thisSubRoot = obj.GetComponent<SubRoot>();
            if (thisSubRoot != null && thisSubRoot.isCyclops)
            {
                if (__result <= 0)
                {
                    int numUpgrades = thisSubRoot.GetCurrentUpgrades().Where(x => x.Contains("FlightModule")).Count();
                    if (numUpgrades > 0)
                    {
                        __result = 10;
                    }
                }
            }
        }
    }
}
