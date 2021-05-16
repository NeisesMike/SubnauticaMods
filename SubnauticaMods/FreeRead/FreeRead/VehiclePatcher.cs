using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System.Net.NetworkInformation;

namespace FreeRead
{
    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Update")]
    class VehiclePatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            if (Input.GetKeyDown(FreeReadPatcher.Config.FreeReadKey))
            {
                FreeReadPatcher.isCruising = true;
                Player.main.GetPDA().Open(PDATab.None, null, null, -1f);
            }

            // add locomotion back in
            if (FreeReadPatcher.isCruising && __instance == Player.main.currentMountedVehicle)
            {
                __instance.GetComponent<Rigidbody>().velocity += __instance.transform.forward * Time.deltaTime * 10f;
                __instance.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(__instance.GetComponent<Rigidbody>().velocity, 10f);
                return false;
            }
            return true;
        }
    }
}
