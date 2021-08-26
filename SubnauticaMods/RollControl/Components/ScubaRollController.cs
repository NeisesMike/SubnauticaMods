using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RollControl
{
    public class ScubaRollController : MonoBehaviour
    {
        public Player player = null;
        private bool isRollReady = false;

        public static bool Swimming
        {
            get
            { 
                return Player.main.motorMode == Player.MotorMode.Dive || Player.main.motorMode == Player.MotorMode.Seaglide;
            }
        }

        private bool isSlowingDown = true;
        private bool isSpeedingUpCW = false;
        private bool isSpeedingUpCCW = false;
        private float fuel = 0;
        private float rollMagnitude = 0;

        private static readonly float MAX_FUEL = 100f;
        private static readonly float MIN_FUEL = 0f;
        private static readonly float SLOW_FUEL_STEP = 10f;
        private static readonly float ACCEL_FUEL_STEP = 25f;
        private static readonly float SCALING_FACTOR = 1f;

        public void FixedUpdate()
        {
            if (RollControlPatcher.Config.ScubaRoll && isRollReady && Swimming && AvatarInputHandler.main.IsEnabled())
            {
                MainCameraControl.main.SetEnabled(false);
                MainCameraControl.main.transform.localRotation = Quaternion.identity;
                PhysicsMouseLook();

                SetupScubaRoll();
                ExecuteScubaRoll();
                CorrectVerticalMovement();
            }
        }
        public void GetReadyToRoll()
        {
            player.transform.rotation = Quaternion.Euler(MainCameraControl.main.transform.eulerAngles.x, MainCameraControl.main.transform.eulerAngles.y, Player.main.transform.rotation.eulerAngles.z);
            // ensure a good mouse-feel
            player.rigidBody.angularDrag = 15;
            // set the camera rotation to zed
            MainCameraControl.main.rotationX = 0; 
            MainCameraControl.main.rotationY = 0;
            isRollReady = true;
        }
        public void GetReadyToStopRolling()
        {
            // re-enable the camera
            MainCameraControl.main.SetEnabled(true);
            isRollReady = false;
        }
        private void ExecuteScubaRoll()
        {
            if (isSlowingDown)
            {
                rollMagnitude = rollMagnitude * (fuel / MAX_FUEL);
                GetComponent<Rigidbody>().AddTorque(player.transform.forward * rollMagnitude, ForceMode.VelocityChange);
                fuel -= SLOW_FUEL_STEP;
                if (fuel <= 0)
                {
                    isSlowingDown = false;
                }
            }
            if (isSpeedingUpCW && isRollReady)
            {
                rollMagnitude = (float)RollControlPatcher.Config.ScubaRollSpeed / 100f * SCALING_FACTOR * (fuel / MAX_FUEL);
                GetComponent<Rigidbody>().AddTorque(player.transform.forward * rollMagnitude, ForceMode.VelocityChange);
                fuel += ACCEL_FUEL_STEP;
            }
            if (isSpeedingUpCCW && isRollReady)
            {
                rollMagnitude = (float)-RollControlPatcher.Config.ScubaRollSpeed / 100f * SCALING_FACTOR * (fuel / MAX_FUEL);
                GetComponent<Rigidbody>().AddTorque(player.transform.forward * rollMagnitude, ForceMode.VelocityChange);
                fuel += ACCEL_FUEL_STEP;
            }
            if (fuel > MAX_FUEL)
            {
                fuel = MAX_FUEL;
            }
            else if (fuel < MIN_FUEL)
            {
                fuel = MIN_FUEL;
            }
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
        public void PhysicsMouseLook()
        {
            Vector2 offset = GameInput.GetLookDelta();
            player.rigidBody.AddTorque(RollControlPatcher.Config.ScubaLookSensitivity / 100f * player.transform.up * offset.x, ForceMode.VelocityChange);
            player.rigidBody.AddTorque(RollControlPatcher.Config.ScubaLookSensitivity / 100f * player.transform.right * -offset.y, ForceMode.VelocityChange);
        }
        public void SetupScubaRoll()
        {
            bool portUp = Input.GetKeyUp(RollControlPatcher.Config.RollPortKey);
            bool portHeld = Input.GetKey(RollControlPatcher.Config.RollPortKey);
            bool portDown = Input.GetKeyDown(RollControlPatcher.Config.RollPortKey);

            bool starUp = Input.GetKeyUp(RollControlPatcher.Config.RollStarboardKey);
            bool starHeld = Input.GetKey(RollControlPatcher.Config.RollStarboardKey);
            bool starDown = Input.GetKeyDown(RollControlPatcher.Config.RollStarboardKey);

            if ((portDown || portHeld) && !(starDown || starHeld))
            {
                startScubaRoll(true);
            }
            else if ((starDown || starHeld) && !(portDown || portHeld))
            {
                startScubaRoll(false);
            }
            else if ((starDown || starHeld) && (portDown || portHeld))
            {
                stopScubaRoll();
            }
            else if ((starUp && !portHeld) || (portUp && !starHeld) || (!portHeld && !starHeld))
            {
                stopScubaRoll();
            }
        }
        public void CorrectVerticalMovement()
        {
            if (GameInput.GetButtonHeld(GameInput.Button.MoveUp) || GameInput.GetButtonHeld(GameInput.Button.MoveDown))
            {
                // hijack the desired velocity
                // Here we cancel out the normal spacebar movement.
                // We reconstruct the vector from scratch,
                // the way Subnautica does it.

                PlayerMotor thisMotor = player.playerController.activeController;

                Vector3 diff = Vector3.zero;
                Vector3 velocity = player.rigidBody.velocity;
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
                    diff = vector3 - player.rigidBody.velocity;
                }

                Vector3 origInverse = new Vector3(0f, -diff.y, 0f);
                // This call to AddForce is what cancels out the original spacebar movement.
                player.rigidBody.AddForce(origInverse, ForceMode.VelocityChange);

                // Now we can make our own movement.
                // We'll do so by constructing a new vector3
                // and adding a force again
                Vector3 myNewVector = player.transform.up * origInverse.magnitude;
                if (GameInput.GetButtonHeld(GameInput.Button.MoveUp) && !GameInput.GetButtonHeld(GameInput.Button.MoveDown))
                {
                    player.rigidBody.AddForce(myNewVector, ForceMode.VelocityChange);
                }
                else if (!GameInput.GetButtonHeld(GameInput.Button.MoveUp) && GameInput.GetButtonHeld(GameInput.Button.MoveDown))
                {
                    player.rigidBody.AddForce(-myNewVector, ForceMode.VelocityChange);
                }
            }
        }
        public void OnSwimmingStarted()
        {
            GetReadyToRoll();
        }
    }
}
