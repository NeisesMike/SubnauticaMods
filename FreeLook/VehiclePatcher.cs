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

namespace FreeLook
{
    public class MyVehicleAccelerationModifier
    {
        public void ModifyAcceleration(ref Vector3 accel)
        {
            accel *= this.accelerationMultiplier;
        }

        [Range(0f, 1f)]
        public float accelerationMultiplier = 0.5f;
    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Update")]

    public class FreeLookPatcher
    {
        public static MyVehicleAccelerationModifier[] myVehicleAccelMods;
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.garyburke.subnautica.freelook.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Initialise();
        }

        public static Options Options = new Options();
        public static void Initialise()
        {
            OptionsPanelHandler.RegisterModOptions(Options);
        }

        public static void moveSeamoth(Vehicle mySeamoth)
        {
            bool flag = mySeamoth.transform.position.y < Ocean.main.GetOceanLevel() && mySeamoth.transform.position.y < mySeamoth.worldForces.waterDepth && !mySeamoth.precursorOutOfWater;
            if (mySeamoth.moveOnLand || flag)
            {
                if (mySeamoth.controlSheme == Vehicle.ControlSheme.Submersible)
                {
                    Vector3 vector = AvatarInputHandler.main.IsEnabled() ? GameInput.GetMoveDirection() : Vector3.zero;
                    vector.Normalize();

                    float leftForce = GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
                    float rightForce = GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
                    float forwardForce = GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
                    float backwardForce = GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
                    float verticalForce = GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp) - GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);

                    leftForce *= 15;
                    rightForce *= 15;
                    forwardForce *= 15;
                    backwardForce *= 15;
                    verticalForce *= 15;

                    float d = Mathf.Abs(vector.x) * rightForce
                            + Mathf.Abs(-vector.x) * leftForce
                            + Mathf.Max(0f, vector.z) * forwardForce
                            + Mathf.Max(0f, -vector.z) * backwardForce
                            + Mathf.Abs(vector.y * verticalForce);

                    Vector3 force = mySeamoth.transform.rotation * (d * vector) * Time.deltaTime;

                    for (int i = 0; i < myVehicleAccelMods.Length; i++)
                    {
                        myVehicleAccelMods[i].ModifyAcceleration(ref force);
                    }

                    mySeamoth.useRigidbody.AddForce(force, ForceMode.VelocityChange);
                    return;
                }
            }
        }
        public static void moveCamera(Vehicle mySeamoth)
        {
            var mainCam = MainCameraControl.main;
            Vector2 vector = GameInput.GetLookDelta();

            mainCam.rotationX += vector.x;
            mainCam.rotationY += vector.y;
            mainCam.rotationY = Mathf.Clamp(mainCam.rotationY, mainCam.minimumY, mainCam.maximumY);
            mainCam.cameraUPTransform.localEulerAngles = new Vector3(Mathf.Min(0f, -mainCam.rotationY), 0f, 0f);
            mainCam.transform.localEulerAngles = new Vector3(Mathf.Max(0f, -mainCam.rotationY), mainCam.rotationX, 0f);
        }

        [HarmonyPrefix]
        public static bool Prefix(Vehicle __instance)
        {
            if(!Player.main.inSeamoth)
            {
                return true;
            }
            if (Input.GetKey(Options.freeLookKey))
            {
                // disable the player controller
                // hack into seamoth movements
                // hack into camera controls
                Player.main.playerController.SetEnabled(true);
                Player.main.playerController.activeController.rb.isKinematic = true;
                Player.main.playerController.activeController.GetComponent<Collider>().enabled = false;
                FPSInputModule.current.lockMovement = false;

                moveCamera(Player.main.currentMountedVehicle);
                moveSeamoth(Player.main.currentMountedVehicle);

                OxygenManager oxygenMgr = Player.main.oxygenMgr;
                oxygenMgr.AddOxygen(1);
                return false;
            }
            if(Input.GetKeyUp(Options.freeLookKey))
            {
                MainCameraControl.main.ResetCamera();
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
        }
    }
}
