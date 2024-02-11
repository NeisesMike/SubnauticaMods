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
using System.Runtime.CompilerServices;
using System.Collections;

namespace RollControlZero
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("StabilizeRoll")]
    class SeaTruckMotorStabilizeRollPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (RollControlPatcher.RCConfig.isSeatruckRollOn)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
