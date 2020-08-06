using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Harmony;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;

namespace RollControl
{
    public class RollManager
    {
        public bool isRollToggled;
        public bool isSlowingDown = true;
        public bool isSpeedingUpCW = false;
        public bool isSpeedingUpCCW = false;

        public RollManager()
        {
            isRollToggled = false;
        }

        public void startScubaRoll(bool isCW)
        {
            if (isCW)
            {
                isSlowingDown = false;
                isSpeedingUpCW = true;
                isSpeedingUpCCW = false;
            }
            else
            {
                isSlowingDown = false;
                isSpeedingUpCW = false;
                isSpeedingUpCCW = true;
            }
        }

        public void stopScubaRoll()
        {
            isSlowingDown = true;
            isSpeedingUpCW = false;
            isSpeedingUpCCW = false;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake")]
    public class PlayerAwakePatcher
    {
        public static RollManager myRollMan;

        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            // initialize the roll manager
            myRollMan = new RollManager();

            return true;
        }
    }
}

