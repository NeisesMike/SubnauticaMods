using HarmonyLib;
using UnityEngine;

namespace RollControl
{
    [HarmonyPatch(typeof(PlayerCinematicController))]
    public class PlayerCinematicControllerPatcher
    {
        public static GameObject cinematicPlayerRotationDummy = null;
        public static Transform endTransform = null;
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerCinematicController.StartCinematicMode))]
        public static bool StartCinematicModePrefix(PlayerCinematicController __instance, Player setplayer)
        {
            if (setplayer.gameObject.EnsureComponent<ScubaRollController>().IsActuallyScubaRolling)
            {
                //Don't want to do this if we're just getting out of our vehicle via moonpool
                endTransform = __instance.endTransform;
                setplayer.gameObject.EnsureComponent<ScubaRollController>().ResetForEndRoll();
            }
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerCinematicController.OnPlayerCinematicModeEnd))]
        public static void OnPlayerCinematicModeEndPostfix(PlayerCinematicController __instance)
        {
            if (endTransform)
            {
                __instance.transform.rotation = endTransform.rotation;
                endTransform = null;
            }
        }
    }
}

