using UnityEngine;
using HarmonyLib;

namespace FreeRead
{
    [HarmonyPatch(typeof(Vehicle))]
    class VehiclePatcher
    {
        [HarmonyPatch(nameof(Vehicle.Awake))]
        [HarmonyPostfix]
        public static void VehicleAwakePostfix(Vehicle __instance)
        {
            __instance.gameObject.EnsureComponent<FreeReadManager>();
        }
        [HarmonyPatch(nameof(Vehicle.Update))]
        [HarmonyPrefix]
        public static bool VehicleUpdatePrefix(Vehicle __instance)
        {
            FreeReadManager frm = __instance.gameObject.EnsureComponent<FreeReadManager>();
            if (Input.GetKeyDown(MainPatcher.FreeReadConfig.FreeReadKey))
            {
                frm.isCruising = true;
                Player.main.GetPDA().Open();
            }

            // add locomotion back in
            if (frm.isCruising && __instance == Player.main.currentMountedVehicle)
            {
                __instance.GetComponent<Rigidbody>().velocity += __instance.transform.forward * Time.deltaTime * 10f;
                __instance.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(__instance.GetComponent<Rigidbody>().velocity, 10f);
                return false;
            }
            return true;
        }
    }
}
