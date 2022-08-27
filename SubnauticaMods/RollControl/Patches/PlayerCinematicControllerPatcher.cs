using HarmonyLib;
using UnityEngine;

namespace RollControl
{
    [HarmonyPatch(typeof(PlayerCinematicController))]
    public class PlayerCinematicControllerPatcher
    {
        public static GameObject cinematicPlayerRotationDummy = null;
        public static Transform endTransform = null;
        private static bool shouldWeResumeRolling = false;

        [HarmonyPrefix]
        [HarmonyPatch("StartCinematicMode")]
        public static bool StartCinematicModePrefix(PlayerCinematicController __instance, Player setplayer)
        {
            endTransform = __instance.endTransform;
            if (ScubaRollController.IsActuallyScubaRolling)
            {
                shouldWeResumeRolling = true;
                __instance.endTransform = null;
                ScubaRollController.ResetForEndRoll();
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("OnPlayerCinematicModeEnd")]
        public static bool OnPlayerCinematicModeEndPrefix(PlayerCinematicController __instance)
        {
            if (shouldWeResumeRolling)
            {
                cinematicPlayerRotationDummy = new GameObject("RollControlCinematicPlayerRotationDummy");
                cinematicPlayerRotationDummy.transform.rotation = Player.main.transform.rotation;
            }
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerCinematicModeEnd")]
        public static void OnPlayerCinematicModeEndPostfix(PlayerCinematicController __instance)
        {
            if (shouldWeResumeRolling)
            {
                ScubaRollController.ResetForStartRoll(cinematicPlayerRotationDummy);
            }
            endTransform = null;
        }
    }
}

