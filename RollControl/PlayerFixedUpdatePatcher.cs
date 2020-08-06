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

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("FixedUpdate")]
    class PlayerFixedUpdatePatcher
    {
        public static float fuel = 0;
        private static float currentVector = 0;

        private static float MAX_FUEL = 100f;
        private static float MIN_FUEL = 0f;
        private static float SLOW_FUEL_STEP = 25f;
        private static float ACCEL_FUEL_STEP = 25f;
        private static float MULTIPLIER = 1f;
        private static float MAX_VECTOR = (float)RollControlPatcher.Options.scubaRollSpeed * MULTIPLIER;


        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if (PlayerAwakePatcher.myRollMan.isSlowingDown)
            {
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * currentVector * (fuel / MAX_FUEL) * (MAX_VECTOR - currentVector)/MAX_VECTOR, ForceMode.VelocityChange);
                fuel -= SLOW_FUEL_STEP;
                if (fuel <= 0)
                {
                    PlayerAwakePatcher.myRollMan.isSlowingDown = false;
                }
            }
            if (PlayerAwakePatcher.myRollMan.isSpeedingUpCW)
            {
                currentVector = (float)RollControlPatcher.Options.scubaRollSpeed * MULTIPLIER * (fuel / MAX_FUEL);
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * currentVector, ForceMode.VelocityChange);
                fuel += ACCEL_FUEL_STEP;
            }
            if (PlayerAwakePatcher.myRollMan.isSpeedingUpCCW)
            {
                currentVector = (float)-RollControlPatcher.Options.scubaRollSpeed * MULTIPLIER * (fuel / MAX_FUEL);
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * currentVector, ForceMode.VelocityChange);
                fuel += ACCEL_FUEL_STEP;
            }

            if ( fuel > MAX_FUEL)
            {
                fuel = MAX_FUEL;
            }
            else if( fuel < MIN_FUEL)
            {
                fuel = MIN_FUEL;
            }

            return true;
        }
    }
}
