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
            foreach(var tmp in __instance.GetComponentsInChildren<Player>())
            {
                if(tmp.GetVehicle() == __instance && tmp.IsPiloting() && tmp.GetComponent<FreeLookManager>().isFreeLooking)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
