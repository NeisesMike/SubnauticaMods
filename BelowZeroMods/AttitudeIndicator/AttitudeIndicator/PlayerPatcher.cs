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
    public class PlayerUpdatePatcher2
    {
        public static GameObject indicator = null;
        public static bool isEnabled = false;

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (!AttitudeIndicatorPatcher.SubnauticaConfig.isAttitudeIndicatorOn)
            {
                if (indicator)
                {
                    GameObject.Destroy(indicator);
                    indicator = null;
                }
                return;
            }
            bool newIsEnabled = __instance.isPiloting && __instance.currentMountedVehicle && !__instance.currentMountedVehicle.name.Contains("Exosuit");
            if (isEnabled != newIsEnabled)
            {
                if (newIsEnabled && AttitudeIndicatorPatcher.SubnauticaConfig.isAttitudeIndicatorOn)
                {
                    if (AttitudeIndicator.prefab is null)
                    {
                        AttitudeIndicator.GetAssets();
                    }
                    indicator = GameObject.Instantiate(AttitudeIndicator.prefab);
                    indicator.AddComponent<AttitudeIndicator>().model = indicator;
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

