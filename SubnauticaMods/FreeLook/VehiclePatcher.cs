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
                if (player.GetVehicle() == __instance && player.IsPiloting() && player.mode == Player.Mode.LockedPiloting && player.GetComponent<FreeLookManager>().isFreeLooking)
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
                if (player.GetVehicle() == __instance && player.IsPiloting() && player.mode == Player.Mode.LockedPiloting && player.GetComponent<FreeLookManager>().isFreeLooking)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
