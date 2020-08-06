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

        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if( PlayerAwakePatcher.myRollMan.shouldResetFuel )
            {
                fuel = 0;
            }


            
            if (PlayerAwakePatcher.myRollMan.isSlowingDown)
            {
                Debug.Log("slow down");
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * currentVector * fuel / 100f, ForceMode.VelocityChange);
                fuel -= 0.25f;
                if (fuel <= 0)
                {
                    PlayerAwakePatcher.myRollMan.isSlowingDown = false;
                }
            }
            if (PlayerAwakePatcher.myRollMan.isSpeedingUpCW)
            {
                Debug.Log("Start Rolling 3");
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * (float)RollControlPatcher.Options.scubaRollSpeed * fuel, ForceMode.VelocityChange);
                fuel += 1;
                currentVector = (float)RollControlPatcher.Options.scubaRollSpeed * fuel;
            }
            if (PlayerAwakePatcher.myRollMan.isSpeedingUpCCW)
            {
                Debug.Log("Start Rolling 3");
                __instance.rigidBody.AddTorque(Camera.main.transform.forward * (float)-RollControlPatcher.Options.scubaRollSpeed * fuel, ForceMode.VelocityChange);
                fuel += 1;
                currentVector = (float)-RollControlPatcher.Options.scubaRollSpeed * fuel;
            }

            if(fuel > 0)
            {
                Debug.Log(fuel);
            }


            if ( fuel > 10 )
            {
                fuel = 10;
            }
            else if( fuel < 0 )
            {
                fuel = 0;
            }


            return true;
        }
    }
}
