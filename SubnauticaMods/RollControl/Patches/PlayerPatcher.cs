using HarmonyLib;
using System.Collections;

namespace RollControl
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void AwakePostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ScubaRollController>();
            Player.main.GetComponent<ScubaRollController>().player = __instance;
            Player.main.GetComponent<ScubaRollController>().isRollEnabled = MainPatcher.RCConfig.IsScubaRollDefaultEnabled;
            if(MainPatcher.RCConfig.IsScubaRollDefaultEnabled)
            {
                Player.main.StartCoroutine(DetermineWhetherWeStartBySwimming());
            }
            Player.main.gameObject.AddComponent<Components.RollHUDElements>();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.UpdateRotation))]
        public static bool Prefix(Player __instance)
        {
            if (Player.main.GetComponent<ScubaRollController>().IsActuallyScubaRolling)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Okay so this actually isn't necessary here, but it's great intel,
        /// so I'm going to let this sleeping dog lie.
        /// </summary>
        /// <returns></returns>
        private static IEnumerator DetermineWhetherWeStartBySwimming()
        {
            // don't do anything until the world is actually loaded
            while (!PAXTerrainController.main || !LargeWorldStreamer.main || !LargeWorldStreamer.main.IsWorldSettled())
            {
                yield return null;
            }
            if (Player.main.GetComponent<ScubaRollController>().AreWeSwimming)
            {
                Player.main.GetComponent<ScubaRollController>().ResetForStartRoll(null);
            }
        }
    }
}

