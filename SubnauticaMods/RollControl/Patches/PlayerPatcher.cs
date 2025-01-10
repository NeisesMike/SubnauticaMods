using HarmonyLib;
using System.Collections;

namespace RollControl
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void AwakePostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ScubaRollController>();
            ScubaRollController.player = __instance;
            ScubaRollController.isRollEnabled = MainPatcher.config.IsScubaRollDefaultEnabled;
            if(MainPatcher.config.IsScubaRollDefaultEnabled)
            {
                Player.main.StartCoroutine(DetermineWhetherWeStartBySwimming());
            }
            Player.main.gameObject.AddComponent<Components.RollHUDElements>();
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateRotation")]
        public static bool Prefix(Player __instance)
        {
            if (ScubaRollController.IsActuallyScubaRolling)
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
            if (ScubaRollController.AreWeSwimming)
            {
                ScubaRollController.ResetForStartRoll(null);
            }
        }
    }
}

