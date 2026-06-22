using UnityEngine;
using HarmonyLib;

namespace RollControl
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
            __result = Mathf.Abs(myAng) <= 38f;
        }
    }
}
