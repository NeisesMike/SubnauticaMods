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
    [HarmonyPatch("Update")]

    public class RollControlPatcher
    {
        public static bool isRollOn = true;
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
            // grab current roll toggle-setting
            if(Input.GetKeyDown(Options.rollToggleKey))
            {
                isRollOn = !isRollOn;
            }

            // seamoth case
            if (__instance.inSeamoth)
            {
                SeamothRoll(__instance, isRollOn);
                return;
            }



            /*
            bool inWater = __instance.activeController == Player.PlayerMotor.;
            bool inSeaGlide = __instance.motorMode == Player.MotorMode.Seaglide;
            bool inPrawn = __instance.motorMode == Player.MotorMode.Mech; // __instance.inExosuit;
            bool inCyclops = Player.main.GetCurrentSub() != null;
            */

            // scuba case
            if (__instance.motorMode == Player.MotorMode.Dive ) //( inWater && !(inSeaGlide || inPrawn || inCyclops) )
            {
                ScubaRoll(__instance, isRollOn);
                return;
            }

            /* I don't know why this is here:
             * it just stops the actor in place
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                FPSInputModule.current.lockMovement = true;
            }
            if (Input.GetKeyUp(KeyCode.PageUp))
            {
                FPSInputModule.current.lockMovement = false;
            }
            */
        }

        public static void SeamothRoll(Player myPlayer, bool roll)
        {
            Vehicle mySeamoth = myPlayer.currentMountedVehicle;
            Debug.Log(mySeamoth.useRigidbody.angularDrag);
            if (roll)
            {
                mySeamoth.stabilizeRoll = false;
            }
            else
            {
                mySeamoth.stabilizeRoll = true;
                return;
            }

            // add roll handlers
            if (Input.GetKey(Options.rollToPortKey))
            {
                mySeamoth.useRigidbody.AddTorque(mySeamoth.transform.forward * (float)Options.seamothRollSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(Options.rollToStarboardKey))
            {
                mySeamoth.useRigidbody.AddTorque(mySeamoth.transform.forward * (float)-Options.seamothRollSpeed, ForceMode.VelocityChange);
            }
        }

        public static void ScubaRoll(Player myPlayer, bool roll)
        {
            //get active player motor
            PlayerMotor thisMotor = myPlayer.playerController.activeController;
            //bool thisCinematicMode = myPlayer.cinematicModeActive;
            //thisMotor.transform.up = myPlayer.transform.up;
            //myPlayer.playerController.forwardReference.rotation = myPlayer.rigidBody.rotation;

            if (roll)
            {
                myPlayer.forceCinematicMode = true;
                thisMotor.rb.freezeRotation = false;
                // this is the same angular drag as the Seamoth's
                myPlayer.rigidBody.angularDrag = 4;
            }
            else
            {
                myPlayer.forceCinematicMode = false;
                thisMotor.rb.freezeRotation = true;
                myPlayer.rigidBody.angularDrag = 0;
                return;
            }

            void updateRots()
            {
                myPlayer.transform.position = thisMotor.transform.position;
                myPlayer.transform.rotation = thisMotor.transform.rotation;
                Camera.main.transform.position = myPlayer.camAnchor.position;
                Camera.main.transform.rotation = thisMotor.transform.rotation;
            }

            // add roll handlers
            if (Input.GetKey(Options.rollToPortKey))
            {
                myPlayer.rigidBody.AddTorque(Camera.main.transform.forward * (float)Options.scubaRollSpeed, ForceMode.VelocityChange);
            }
            else if (Input.GetKey(Options.rollToStarboardKey))
            {
                myPlayer.rigidBody.AddTorque(Camera.main.transform.forward * (float)-Options.scubaRollSpeed, ForceMode.VelocityChange);
            }
        }
    }
}

