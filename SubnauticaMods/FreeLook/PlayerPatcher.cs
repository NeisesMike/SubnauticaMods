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
using LitJson;
using System.Net.NetworkInformation;

namespace FreeLook
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Start")]
    public class PlayerStartPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            return true;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        private static bool isRelinquished = false;

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if(Player.main.currentMountedVehicle == null || Player.main.currentMountedVehicle.docked)
            {
                return;
            }
            bool inVehicleThisFrame = (__instance.inSeamoth || __instance.inExosuit);
            if(!inVehicleThisFrame && !isRelinquished)
            {
                FreeLookManager.cameraRelinquish();
                isRelinquished = true;
            }
            else if(inVehicleThisFrame)
            {
                isRelinquished = false;
            }
        }
    }
}