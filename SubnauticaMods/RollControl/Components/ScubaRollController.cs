using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SMLHelper.V2.Utility;

namespace RollControl
{
    public class ScubaRollController : MonoBehaviour
    {
        public static bool isRollEnabled = RollControlPatcher.Config.IsScubaRollDefaultEnabled;
        public static Player player = null;

        private static MainCameraControl _CamControl = MainCameraControl.main;
        public static MainCameraControl CamControl
        {
            get
            {
                if(_CamControl == null)
                {
                    _CamControl = MainCameraControl.main;
                    return _CamControl;
                }
                return _CamControl;
            }
        }

        public static Camera camera;
        public static bool isRollReady = false;
        public static bool wasRollingBeforeBuilder = false;

        public static ScubaRollController main;

        public enum SwimState
        {
            Dive,
            Surface,
            Breach,
            RunFall
        }
        public static SwimState swimstate;

        public static bool IsActuallyScubaRolling
        {
            get
            {
                return
                    isRollEnabled &&
                    isRollReady &&
                    AreWeSwimming;
            }
        }
        public static bool IsAbleToStartScubaRolling
        {
            get
            {
                return
                    IsAbleToToggleScubaRolling &&
                    isRollEnabled;
            }
        }
        public static bool IsAbleToToggleScubaRolling
        {
            get
            {
                return
                    AreWeSwimming &&
                    AvatarInputHandler.main.IsEnabled();
            }
        }
        public static bool AreWeSwimming
        {
            get
            {
                return
                    swimstate != SwimState.RunFall &&
                    (Player.main.transform.position.y < Ocean.main.GetOceanLevel() || Player.main.GetComponent<UnderwaterMotor>().isActiveAndEnabled || swimstate == SwimState.Breach);
                    //Player.main.GetComponent<UnderwaterMotor>().isActiveAndEnabled; // This not works because not available at Start time
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

        public static void ResetRotsForStartRoll()
        {
            player.transform.eulerAngles = new Vector3(-CamControl.rotationY, CamControl.rotationX, 0);
            player.transform.Find("body").localEulerAngles = Vector3.zero;
            CamControl.transform.localEulerAngles = Vector3.zero;
            CamControl.transform.Find("camOffset").localEulerAngles = Vector3.zero;
            CamControl.rotationX = 0;
            CamControl.rotationY = 0;
        }
        public static void ResetRotsForStartRollAfterCinematic(Quaternion desiredRotation)
        {
            player.transform.rotation = desiredRotation;
            player.transform.Find("body").localEulerAngles = Vector3.zero;
            CamControl.transform.localEulerAngles = Vector3.zero;
            CamControl.transform.Find("camOffset").localEulerAngles = Vector3.zero;
            CamControl.rotationX = 0;
            CamControl.rotationY = 0;
        }
        public static void ResetForStartRoll(GameObject lookTarget)
        {
            player.rigidBody.angularDrag = 15;
            main.StartCoroutine(PauseCameraOneFrame());
            if (lookTarget)
            {
                ResetRotsForStartRollAfterCinematic(lookTarget.transform.rotation);
            }
            else
            {
                ResetRotsForStartRoll();
            }
            isRollReady = true;
        }
        public static void ResetForEndRoll()
        {
            GameObject look_target = new GameObject("look target");
            look_target.transform.position = player.transform.position + player.transform.forward;
            main.StartCoroutine(PauseCameraOneFrame());
            player.transform.LookAt(look_target.transform, Vector3.up);
            CamControl.rotationX = player.transform.eulerAngles.y;
            float pitchOffset = player.transform.eulerAngles.x;
            if (pitchOffset < 180)
            {
                CamControl.rotationY = -pitchOffset;
            }
            else
            {
                CamControl.rotationY = 360 - pitchOffset;
            }
            player.transform.rotation = Quaternion.identity;
            CamControl.transform.localRotation = Quaternion.identity;
            isRollReady = false;
        }
        public void Start()
        {
            main = this;
            camera = CamControl?.transform.Find("camOffset/pdaCamPivot/PlayerCameras/MainCamera")?.GetComponent<Camera>();
        }
        public void Update()
        {
            if (camera==null)
            {
                camera = CamControl?.transform.Find("camOffset/pdaCamPivot/PlayerCameras/MainCamera")?.GetComponent<Camera>();
            }
            SwimState lastSwimState = swimstate;
            swimstate = DetermineSwimState();
            CheckForUserToggleInput();
            HandleSwimStateChange(lastSwimState);
        }
        private void HandleSwimStateChange(SwimState lastSwimState)
        {
            if (isRollEnabled && lastSwimState != swimstate)
            {
                if (lastSwimState == SwimState.RunFall && IsAbleToToggleScubaRolling)
                {
                    // transition into a scuba roll control state
                    ResetForStartRoll(null);
                }
                if (swimstate == SwimState.RunFall)
                {
                    // transition into a normal camera state
                    ResetForEndRoll();
                }
            }
        }
        private SwimState DetermineSwimState()
        {
            if (Player.main.motorMode == Player.MotorMode.Dive || Player.main.motorMode == Player.MotorMode.Seaglide)
            {
                return SwimState.Dive;
            }
            if (!Player.main.GetComponent<GroundMotor>().IsGrounded() && (swimstate == SwimState.Surface || swimstate == SwimState.Breach))
            {
                return SwimState.Breach;
            }
            if (Player.main.motorMode == Player.MotorMode.Run && Player.main.IsUnderwaterForSwimming())
            {
                return SwimState.Surface;
            }
            return SwimState.RunFall;
        }
        private void CheckForUserToggleInput()
        {
            if (IsAbleToToggleScubaRolling)
            {
                if (Input.GetKeyDown(RollControlPatcher.Config.ToggleRollKey))
                {
                    if (isRollEnabled)
                    {
                        isRollEnabled = false;
                        ResetForEndRoll();
                    }
                    else
                    {
                        ResetForStartRoll(null);
                        isRollEnabled = true;
                    }
                }
            }
        }
        private static IEnumerator PauseCameraOneFrame()
        {
            Camera myCam = Camera.main;
            if(myCam is null)
            {
                yield break;
            }
            myCam.enabled = false;
            yield return null;
            myCam.enabled = true;
        }
        public void FixedUpdate()
        {
            if (IsActuallyScubaRolling && AvatarInputHandler.main.IsEnabled())
            {
                CamControl.transform.localRotation = Quaternion.identity;
                PhysicsMouseLook();
                SetupScubaRoll();
                ExecuteScubaRoll();
                CorrectVerticalMovement();
            }
        }
        public void GetReadyToStopRolling()
        {
            isRollReady = false;
        }
        private void ExecuteScubaRoll()
        {
            if (isSlowingDown)
            {
                rollMagnitude *= (fuel / MAX_FUEL);
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
        public void StartScubaRoll(bool isCW)
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
        public void StopScubaRoll()
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
                StartScubaRoll(true);
            }
            else if ((starDown || starHeld) && !(portDown || portHeld))
            {
                StartScubaRoll(false);
            }
            else if ((starDown || starHeld) && (portDown || portHeld))
            {
                StopScubaRoll();
            }
            else if ((starUp && !portHeld) || (portUp && !starHeld) || (!portHeld && !starHeld))
            {
                StopScubaRoll();
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

                // TODO
                // There is a problem here.
                // Namely, Subnautica normally disallows the player from looking straight up or straight down
                // (to, I think, 5 degrees of tolerance)
                // so it stands to reason that when we try to "recreate" the OG vector in this "impossible" situation,
                // we fail in strange ways.

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
                //Vector3 vector2 = thisMotor.playerController.forwardReference.rotation * vector; // this was not the problem
                Vector3 vector2 = Player.main.transform.rotation * vector; // this was not the solution
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
                    Vector3 vector3 = velocity + vector * num5; // maybe this is the problom?
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

        // MaybeSetLocalEuler is used in a MainCameraControl transpiler
        public static void MaybeSetLocalEuler(Transform camTrans, Vector3 input)
        {
            if(swimstate == SwimState.RunFall || !isRollEnabled || Builder.isPlacing)
            {
                camTrans.localEulerAngles = input;
            }
        }
    }
}
