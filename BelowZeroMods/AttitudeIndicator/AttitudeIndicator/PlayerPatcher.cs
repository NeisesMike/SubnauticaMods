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
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;

namespace AttitudeIndicator
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        public static float lastUpdateTime = Time.time;

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            bool shouldWeUpdateLadder = false;
#if SUBNAUTICA
            if (lastUpdateTime + AttitudeIndicatorPatcher.SubnauticaConfig.updateInterval < Time.time)
            {
                lastUpdateTime = Time.time;
                shouldWeUpdateLadder = true;
            }
#elif BELOWZERO
            if (lastUpdateTime + AttitudeIndicatorPatcher.BelowZeroConfig.updateInterval < Time.time)
            {
                lastUpdateTime = Time.time;
                shouldWeUpdateLadder = true;
            }
#endif


#if SUBNAUTICA
            if (!__instance.isPiloting)
            {
                AttitudeIndicator.killAttitudeIndicator();
                AttitudeIndicatorPatcher.currentVehicle = VehicleType.None;
                return;
            }
            if (__instance.currentMountedVehicle)
            {
                if (!__instance.currentMountedVehicle.name.Contains("Exosuit"))
                {
                    AttitudeIndicatorPatcher.currentVehicle = VehicleType.Seamoth;
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.SisAttitudeIndicatorOn)
                    {
                        if (AttitudeIndicator.AttitudeIndicatorScreen == null)
                        {
                            AttitudeIndicator.createAttitudeIndicator(VehicleType.Seamoth);
                        }
                        AttitudeIndicator.updateAttitudeIndicator(VehicleType.Seamoth, shouldWeUpdateLadder);
                    }
                    else if (AttitudeIndicator.AttitudeIndicatorScreen != null)
                    {
                        AttitudeIndicator.killAttitudeIndicator();
                    }
                }
            }
            else if (__instance.GetCurrentSub())
            {
                if (__instance.GetCurrentSub().name.Contains("Cyclops"))
                {
                    AttitudeIndicatorPatcher.currentVehicle = VehicleType.Cyclops;
                    if (AttitudeIndicatorPatcher.SubnauticaConfig.CisAttitudeIndicatorOn)
                    {
                        if (AttitudeIndicator.AttitudeIndicatorScreen == null)
                        {
                            AttitudeIndicator.createAttitudeIndicator(VehicleType.Cyclops);
                        }
                        AttitudeIndicator.updateAttitudeIndicator(VehicleType.Cyclops, shouldWeUpdateLadder);
                    }
                    else if (AttitudeIndicator.AttitudeIndicatorScreen != null)
                    {
                        AttitudeIndicator.killAttitudeIndicator();
                    }
                }
            }
#elif BELOWZERO
            if (!(__instance.IsPilotingSeatruck() || __instance.inHovercraft))
            {
                AttitudeIndicator.killAttitudeIndicator();
                AttitudeIndicatorPatcher.currentVehicle = VehicleType.None;
                return;
            }
            if (__instance.IsPilotingSeatruck())
            {
                AttitudeIndicatorPatcher.currentVehicle = VehicleType.Seatruck;
                if (AttitudeIndicatorPatcher.BelowZeroConfig.TisAttitudeIndicatorOn)
                {
                    if (AttitudeIndicator.AttitudeIndicatorScreen == null)
                    {
                        AttitudeIndicator.createAttitudeIndicator(VehicleType.Seatruck);
                    }
                    AttitudeIndicator.updateAttitudeIndicator(VehicleType.Seatruck, shouldWeUpdateLadder);
                }
                else if (AttitudeIndicator.AttitudeIndicatorScreen != null)
                {
                    AttitudeIndicator.killAttitudeIndicator();
                }
            }
            else if (__instance.inHovercraft)
            {
                AttitudeIndicatorPatcher.currentVehicle = VehicleType.Snowfox;
                if (AttitudeIndicatorPatcher.BelowZeroConfig.FisAttitudeIndicatorOn)
                {
                    if (AttitudeIndicator.AttitudeIndicatorScreen == null)
                    {
                        AttitudeIndicator.createAttitudeIndicator(VehicleType.Snowfox);
                    }
                    AttitudeIndicator.updateAttitudeIndicator(VehicleType.Snowfox, shouldWeUpdateLadder);
                }
                else if (AttitudeIndicator.AttitudeIndicatorScreen != null)
                {
                    AttitudeIndicator.killAttitudeIndicator();
                }
            }
#endif
            else if (AttitudeIndicator.AttitudeIndicatorScreen != null)
            {
                AttitudeIndicator.killAttitudeIndicator();
            }
        }
    }
}

