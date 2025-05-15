using UnityEngine;
using HarmonyLib;

namespace FreeRead
{
    [HarmonyPatch(typeof(Vehicle))]
    class VehiclePatcher
    {
        private static FreeReadManager.InputPair StronglyEnableInput()
        {
            var result = new FreeReadManager.InputPair
            {
                avatarEnabled = AvatarInputHandler.main.gameObject.activeSelf,
                lockCursor = UWE.Utils.lockCursor
            };
            UWE.Utils.lockCursor = true;
            AvatarInputHandler.main.gameObject.SetActive(true);
            return result;
        }
        private static void StronglyRememberInput(FreeReadManager.InputPair inputPair)
        {
            UWE.Utils.lockCursor = inputPair.lockCursor;
            AvatarInputHandler.main.gameObject.SetActive(inputPair.avatarEnabled);
        }

        [HarmonyPatch(nameof(Vehicle.Update))]
        [HarmonyPostfix]
        public static void VehicleUpdatePostfix(Vehicle __instance)
        {
            FreeReadManager frm = __instance.gameObject.EnsureComponent<FreeReadManager>();
            if (Input.GetKeyDown(MainPatcher.FreeReadConfig.FreeReadKey))
            {
                frm.isFreeReading = !frm.isFreeReading;
                if (frm.isFreeReading)
                {
                    Player.main.GetPDA().Open();
                }
            }
            if(__instance is SeaMoth sm)
            {
                var inputPair = StronglyEnableInput();
                sm.UpdateSounds();
                StronglyRememberInput(inputPair);
            }
        }

        [HarmonyPatch(nameof(Vehicle.FixedUpdate))]
        [HarmonyPrefix]
        public static void VehicleFixedUpdatePrefix(Vehicle __instance)
        {
            var frm = __instance.gameObject.EnsureComponent<FreeReadManager>();
            if (frm.isFreeReading)
            {
                frm.lastInputPair = StronglyEnableInput();
            }
        }

        [HarmonyPatch(nameof(Vehicle.FixedUpdate))]
        [HarmonyPostfix]
        public static void VehicleFixedUpdatePostfix(Vehicle __instance)
        {
            var frm = __instance.gameObject.EnsureComponent<FreeReadManager>();
            if (frm.isFreeReading)
            {
                StronglyRememberInput(frm.lastInputPair);
            }
        }
    }
}
