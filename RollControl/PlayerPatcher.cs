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
        public static bool isSeamothRollOn = false;
        public static bool isScubaRollOn   = false;

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
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            // grab current roll toggle-setting
            if(Input.GetKeyDown(Options.seamothRollToggleKey))
            {
                isSeamothRollOn = !isSeamothRollOn;
            }
            if (Input.GetKeyDown(Options.scubaRollToggleKey))
            {
                isScubaRollOn = !isScubaRollOn;
            }

            if (__instance.inSeamoth)
            {
                SeamothRoll(__instance, isSeamothRollOn);
                return;
            }
            else if (__instance.motorMode == Player.MotorMode.Dive )
            {
                ScubaRoll(__instance, isScubaRollOn);
                return;
            }
        }

        public static void SeamothRoll(Player myPlayer, bool roll)
        {
            Vehicle mySeamoth = myPlayer.currentMountedVehicle;
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
            UnderwaterMotor thisMotor = (UnderwaterMotor)myPlayer.playerController.activeController;

            if (roll)
            {
                myPlayer.forceCinematicMode = true;
                thisMotor.rb.freezeRotation = false;
                if(Options.scubaRollUnlimited)
                {
                    MainCameraControl.main.minimumY = -10000f;
                    MainCameraControl.main.maximumY = 10000f;
                }
                else
                {
                    MainCameraControl.main.minimumY = -80f;
                    MainCameraControl.main.maximumY = 80f;
                }
                // this is the same angular drag as the Seamoth's
                myPlayer.rigidBody.angularDrag = 4;
            }
            else
            {
                myPlayer.forceCinematicMode = false;
                thisMotor.rb.freezeRotation = true;
                MainCameraControl.main.minimumY = -80f;
                MainCameraControl.main.maximumY = 80f;
                myPlayer.rigidBody.angularDrag = 0;
                return;
            }

            void updateRots()
            {
                Rigidbody proper = myPlayer.rigidBody;
                myPlayer.transform.position = proper.position;
                myPlayer.transform.rotation = proper.rotation;
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
            updateRots();

            if (Input.GetKey(KeyCode.Space))
            {
                // hijack the desired velocity

                /*
                 * Here we cancel out the normal spacebar movement.
                 * We reconstruct the vector from scratch,
                 * the way Subnautica does it.
                 * I kept their naming conventions, sorry.
                 */
                Vector3 diff = Vector3.zero;
                Vector3 velocity = myPlayer.rigidBody.velocity;
                Vector3 vector = GameInput.GetMoveDirection();
                float y = vector.y;
                float num = Mathf.Min(1f, vector.magnitude);
                vector.y = 0f;
                vector.Normalize();
                float num2 = 0f;
                if (vector.z > 0f)
                {
                    num2 = thisMotor.forwardMaxSpeed;
                }
                else if (vector.z < 0f)
                {
                    num2 = -thisMotor.backwardMaxSpeed;
                }
                if (vector.x != 0f)
                {
                    num2 = Mathf.Max(num2, thisMotor.strafeMaxSpeed);
                }
                num2 = Mathf.Max(num2, thisMotor.verticalMaxSpeed);
                num2 *= Player.main.mesmerizedSpeedMultiplier;
                float num3 = Mathf.Max(velocity.magnitude, num2);
                Vector3 vector2 = thisMotor.playerController.forwardReference.rotation * vector;
                vector = vector2;
                vector.y += y;
                vector.Normalize();

                float num4 = thisMotor.acceleration;
                if (Player.main.GetBiomeString() == "wreck")
                {
                    num4 *= 0.5f;
                }
                float num5 = num * num4 * Time.deltaTime;
                if (num5 > 0f)
                {
                    Vector3 vector3 = velocity + vector * num5;
                    if (vector3.magnitude > num3)
                    {
                        vector3.Normalize();
                        vector3 *= num3;
                    }
                    diff = vector3 - myPlayer.rigidBody.velocity;
                    //myPlayer.rigidBody.velocity = vector3;
                }

                Vector3 origInverse = new Vector3(0f, -diff.y, 0f);
                // This call to AddForce is what cancels out the original spacebar movement.
                myPlayer.rigidBody.AddForce(origInverse, ForceMode.VelocityChange);

                // Now we can make our own movement.
                // We'll do so by constructing a new vector3
                // and adding a force again
               
                // I think this is already normal, but playing it safe
                Vector3 myDirection = Camera.main.transform.up.normalized;
                float myMagnitude = origInverse.magnitude;
                Vector3 myNewVector = myDirection * myMagnitude;

                myPlayer.rigidBody.AddForce(myNewVector, ForceMode.VelocityChange);
            }
        }
    }
}

