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

namespace RollControl
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(Player __instance)
        {
            var srm = __instance.gameObject.EnsureComponent<ScubaRollController>();
            srm.player = __instance;
        }

        /*
         * Here we do all the normal stuff except for a case where we were falling last frame
         * Since we're underwater, there's no chance we were falling last frame
         * They key here is that we can use this function to override and delete the og FixedUpdate,
         * which is what lerps the player rotations back to normal
         */
        [HarmonyPrefix]
        [HarmonyPatch("UpdateRotation")]
        public static bool Prefix(Player __instance)
        {
            if (__instance.GetComponent<ScubaRollController>().isRollToggled && __instance.IsUnderwater())
            {
                return false;
            }
            return true;
        }
    }
}

