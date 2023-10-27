using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using System.Runtime.CompilerServices;
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
            ScubaRollController.isRollEnabled = RollControlPatcher.config.IsScubaRollDefaultEnabled;
            if(RollControlPatcher.config.IsScubaRollDefaultEnabled)
            {
                Player.main.StartCoroutine(DetermineWhetherWeStartBySwimming());
            }
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

