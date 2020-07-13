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

namespace RollControl
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]

    public class RollControlPatcher
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.garyburke.subnautica.rollcontrol.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Initialise();
        }

        public static Options Options = new Options();
        public static void Initialise()
        {
            OptionsPanelHandler.RegisterModOptions(Options);
        }

        [HarmonyPrefix]
        public static bool Prefix(Player __instance)
        {
            if (__instance.inSeamoth)
            {

            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            // be sure to only do this for the seamoth
            if (!__instance.inSeamoth)
            {
                return;
            }

            // disable roll stabilization
            var myVehicle = __instance.currentMountedVehicle;
            myVehicle.stabilizeRoll = false;

            // add roll handlers
            if (Input.GetKey(Options.rollToPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * Options.rollSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(Options.rollToStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * -Options.rollSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                FPSInputModule.current.lockMovement = true;
            }
            if (Input.GetKeyUp(KeyCode.PageUp))
            {
                FPSInputModule.current.lockMovement = false;
            }
        }
    }
}

