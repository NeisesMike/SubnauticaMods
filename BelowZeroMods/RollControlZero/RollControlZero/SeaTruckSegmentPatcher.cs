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

namespace RollControlZero
{
    [HarmonyPatch(typeof(SeaTruckSegment))]
    [HarmonyPatch("IsWalkable")]
    class SeaTruckSegmentIsWalkablePatcher
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckSegment __instance, ref bool __result)
        {
            Quaternion truckQuat = __instance.transform.rotation;
            truckQuat.eulerAngles = new Vector3(truckQuat.eulerAngles.x, 0, truckQuat.eulerAngles.z);
            float myAng = Quaternion.Angle(truckQuat, Quaternion.identity);
            Logger.Output(myAng.ToString());
            __result = Mathf.Abs(myAng) <= 38f;
        }
    }
}
