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
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;

namespace RollControlZero
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (Input.GetKeyDown(RollControlPatcher.Config.rollToggleKey))
            {
                RollControlPatcher.Config.isSeatruckRollOn = !RollControlPatcher.Config.isSeatruckRollOn;
            }
            /*
            if (Input.GetKeyDown(Options.scubaRollToggleKey))
            {
                isScubaRollOn = !isScubaRollOn;
                PlayerAwakePatcher.myRollMan.isRollToggled = !PlayerAwakePatcher.myRollMan.isRollToggled;
            }
            */

            if (__instance.IsPilotingSeatruck())
            {
                if (RollControlPatcher.Config.isHUD)
                {
                    Hint main = Hint.main;
                    if (main == null)
                    {
                        return;
                    }
                    uGUI_PopupMessage message = main.message;
                    message.ox = 60f;
                    message.oy = 0f;
                    message.anchor = RollControlPatcher.Config.HUDAnchor;
                    message.SetBackgroundColor(new Color(1f, 1f, 1f, 1f));
                    double myPitch = Math.Truncate(__instance.transform.eulerAngles.x);
                    double myYaw = Math.Truncate(__instance.transform.eulerAngles.y);
                    double myRoll = Math.Truncate(__instance.transform.eulerAngles.z);
                    string myMessage = "Pitch: " + myPitch + "\nRoll: " + myRoll + "\nYaw:" + myYaw;
                    message.SetText(myMessage, RollControlPatcher.Config.HUDAnchor);
                    message.Show(3f, 0f, 0.25f, 0.25f, null);
                }
                if (RollControlPatcher.Config.isSeatruckRollOn)
                {
                    SeatruckRoll();
                }
            }

            /*
            else if (__instance.motorMode == Player.MotorMode.Dive || __instance.motorMode == Player.MotorMode.Seaglide)
            {
                Logger.Log("We're scuba rolling!");
                ScubaRoll(__instance, isScubaRollOn);
                return;
            }
            */
        }

        public static void SeatruckRoll()
        {
            SeaTruckMotor seaTruckMotor = Player.main.GetComponentInParent<SeaTruckMotor>();
            float rollFactor = RollControlPatcher.Config.seatruckRollSpeed / 100.0f;

            // add roll handlers
            if (Input.GetKey(RollControlPatcher.Config.rollToPortKey))
            {
                seaTruckMotor.useRigidbody.AddTorque(seaTruckMotor.transform.forward * rollFactor, ForceMode.VelocityChange);
            }
            if (Input.GetKey(RollControlPatcher.Config.rollToStarboardKey))
            {
                seaTruckMotor.useRigidbody.AddTorque(seaTruckMotor.transform.forward * -rollFactor, ForceMode.VelocityChange);
            }
        }


        public static void ScubaRoll(Player myPlayer, bool roll)
        {
            //get active player motor
            UnderwaterMotor thisMotor = (UnderwaterMotor)myPlayer.playerController.activeController;

            if (roll)
            {
                if (false)//Options.scubaRollUnlimited)
                {
                    MainCameraControl.main.minimumY = -10000f;
                    MainCameraControl.main.maximumY = 10000f;
                }
                else
                {
                    MainCameraControl.main.minimumY = -80f;
                    MainCameraControl.main.maximumY = 80f;
                }

                // this is the same angular drag as the Seamoth's: 4
                myPlayer.rigidBody.angularDrag = 4;
            }
            else
            {
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
            bool portUp = Input.GetKeyUp(RollControlPatcher.Config.rollToPortKey);
            bool portHeld = Input.GetKey(RollControlPatcher.Config.rollToPortKey);
            bool portDown = Input.GetKeyDown(RollControlPatcher.Config.rollToPortKey);

            bool starUp = Input.GetKeyUp(RollControlPatcher.Config.rollToPortKey);
            bool starHeld = Input.GetKey(RollControlPatcher.Config.rollToPortKey);
            bool starDown = Input.GetKeyDown(RollControlPatcher.Config.rollToPortKey);

            if (portDown || portHeld)
            {
                PlayerAwakePatcher.myRollMan.startScubaRoll(true);
            }
            else if (starDown || starHeld)
            {
                PlayerAwakePatcher.myRollMan.startScubaRoll(false);
            }
            else if ((starUp && !portHeld) || (portUp && !starHeld) || (!portHeld && !starHeld))
            {
                PlayerAwakePatcher.myRollMan.stopScubaRoll();
            }

            updateRots();

            if (GameInput.GetButtonHeld(GameInput.Button.MoveUp) || GameInput.GetButtonHeld(GameInput.Button.MoveDown))
            {
                // hijack the desired velocity


                // Here we cancel out the normal spacebar movement.
                // We reconstruct the vector from scratch,
                // the way Subnautica does it.
                // I kept their naming conventions, sorry.

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

                float num4 = thisMotor.waterAcceleration;
                if (Player.main.GetBiomeString() == "wreck")
                {
                    num4 *= 0.5f;
                }
                else if (Player.main.motorMode == Player.MotorMode.Seaglide)
                {
                    num4 *= 1.45f;
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

                // if we're in here, we're either going up or down
                if (GameInput.GetButtonHeld(GameInput.Button.MoveUp))
                {
                    myPlayer.rigidBody.AddForce(myNewVector, ForceMode.VelocityChange);
                }
                else
                {
                    myPlayer.rigidBody.AddForce(-myNewVector, ForceMode.VelocityChange);
                }
            }
        }
    }
}

