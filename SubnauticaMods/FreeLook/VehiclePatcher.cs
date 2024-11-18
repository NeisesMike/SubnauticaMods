using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace FreeLook
{
    [HarmonyPatch(typeof(Vehicle))]
    public static class VehiclePatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool UpdatePrefix(Vehicle __instance)
        {
            foreach(var player in __instance.GetComponentsInChildren<Player>())
            {
                if (FreeLookManager.ShouldDoEngineAction && player.GetVehicle() == __instance)
                {
                    return false;
                }
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("FixedUpdate")]
        public static bool FixedUpdatePrefix(Vehicle __instance)
        {
            foreach (var player in __instance.GetComponentsInChildren<Player>())
            {
                if(FreeLookManager.ShouldDoEngineAction && player.GetVehicle() == __instance)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
