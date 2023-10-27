using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Utility;

namespace AttitudeIndicator
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher2
    {
        public static GameObject indicator = null;
        public static bool isEnabled = false;

        public static bool IsInSeamoth
        {
            get
            {
                return Player.main.isPiloting && Player.main.currentMountedVehicle && !Player.main.currentMountedVehicle.name.Contains("Exosuit");
            }
        }
        public static bool IsPilotCyclop
        {
            get
            {
                return Player.main.isPiloting && Player.main.GetCurrentSub() && Player.main.GetCurrentSub().name.Contains("Cyclops");
            }
        }
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (!AttitudeIndicatorPatcher.SubnauticaConfig.isCyclopsAttitudeIndicatorOn && !AttitudeIndicatorPatcher.SubnauticaConfig.isSeamothAttitudeIndicatorOn)
            {
                if (indicator)
                {
                    GameObject.Destroy(indicator);
                    indicator = null;
                }
                return;
            }

            bool newIsEnabled = IsInSeamoth || IsPilotCyclop;

            if (isEnabled != newIsEnabled)
            {
                if(IsInSeamoth && AttitudeIndicatorPatcher.SubnauticaConfig.isSeamothAttitudeIndicatorOn
                    || IsPilotCyclop && AttitudeIndicatorPatcher.SubnauticaConfig.isCyclopsAttitudeIndicatorOn)
                {
                    if (AttitudeIndicator.prefab is null)
                    {
                        AttitudeIndicator.GetAssets();
                    }
                    indicator = GameObject.Instantiate(AttitudeIndicator.prefab);
                    indicator.AddComponent<AttitudeIndicator>().model = indicator;
                    if(IsInSeamoth)
                    {
                        AttitudeIndicatorPatcher.currentVehicle = VehicleType.Seamoth;
                    }
                    else if(IsPilotCyclop)
                    {
                        AttitudeIndicatorPatcher.currentVehicle = VehicleType.Cyclops;
                    }
                }
                else
                {
                    if (indicator)
                    {
                        GameObject.Destroy(indicator);
                        indicator = null;
                    }
                }
            }
            isEnabled = newIsEnabled;
        }
    }
}

