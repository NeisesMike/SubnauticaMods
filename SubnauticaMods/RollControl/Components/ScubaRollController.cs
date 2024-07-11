using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Utility;

namespace RollControl
{
    public class ScubaRollController : MonoBehaviour
    {
        public static bool isRollEnabled = RollControlPatcher.config.IsScubaRollDefaultEnabled;
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
                    (Player.main.transform.position.y < Ocean.GetOceanLevel() || Player.main.GetComponent<UnderwaterMotor>().isActiveAndEnabled || swimstate == SwimState.Breach);
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
        private static readonly float SCALING_FACTOR = 4f;

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

                    // remind the user that scuba roll is enabled, show them the toggle key
                    Logger.Output("Scuba Roll is ON. Toggle with " + RollControlPatcher.config.ToggleRollKey);
                    SetupForScubaRollOnceAtStart();
                }
                if (swimstate == SwimState.RunFall)
                {
                    // transition into a normal camera state
                    ResetForEndRoll();

                    SetupEndingScubaRollOnceAtExit();
                }
            }
        }
        public void SetupForScubaRollOnceAtStart()
        {
            player.rigidBody.freezeRotation = false;
            player.GetComponent<Collider>().material.frictionCombine = PhysicMaterialCombine.Minimum;
            player.GetComponent<Collider>().material.dynamicFriction = 0;
            player.GetComponent<Collider>().material.staticFriction = 0;
            player.rigidBody.angularDrag = 7.5f; // By default, it's 15
            player.rigidBody.inertiaTensor = Vector3.one;
        }
        public void SetupEndingScubaRollOnceAtExit()
        {
            player.rigidBody.freezeRotation = true;
            player.GetComponent<Collider>().material.frictionCombine = PhysicMaterialCombine.Average;
            player.GetComponent<Collider>().material.dynamicFriction = 0.6f;
            player.GetComponent<Collider>().material.staticFriction = 0.6f;
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
                if (Input.GetKeyDown(RollControlPatcher.config.ToggleRollKey))
                {
                    if (isRollEnabled)
                    {
                        isRollEnabled = false;
                        ResetForEndRoll();
                        SetupEndingScubaRollOnceAtExit();
                    }
                    else
                    {
                        ResetForStartRoll(null);
                        isRollEnabled = true;
                        SetupForScubaRollOnceAtStart();
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
            /*
            RollControlPatcher.logger.LogWarning("isactually " + IsActuallyScubaRolling.ToString());
            RollControlPatcher.logger.LogWarning("player mass " + player.rigidBody.mass.ToString());
            RollControlPatcher.logger.LogWarning("player angdrag " + player.rigidBody.angularDrag.ToString());
            RollControlPatcher.logger.LogWarning("ire " + isRollEnabled.ToString());
            RollControlPatcher.logger.LogWarning("irr " + isRollReady.ToString());
            RollControlPatcher.logger.LogWarning("aws " + AreWeSwimming.ToString());
            RollControlPatcher.logger.LogWarning("player mass " + player.rigidBody.mass.ToString());
            RollControlPatcher.logger.LogWarning("player angdrag " + player.rigidBody.angularDrag.ToString());
            RollControlPatcher.logger.LogWarning("player freeze " + player.rigidBody.freezeRotation.ToString());
            RollControlPatcher.logger.LogWarning("player maxang " + player.rigidBody.maxAngularVelocity.ToString());
            RollControlPatcher.logger.LogWarning("player itr " + player.rigidBody.inertiaTensorRotation.ToString());
            */
            if (IsActuallyScubaRolling && AvatarInputHandler.main.IsEnabled())
            {
                CamControl.transform.localRotation = Quaternion.identity;
                PhysicsMouseLook();
                SetupScubaRoll();
                ExecuteScubaRoll();
            }
            else
            {
                player.rigidBody.freezeRotation = true;
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
                rollMagnitude = (float)RollControlPatcher.config.ScubaRollSpeed / 100f * SCALING_FACTOR * (fuel / MAX_FUEL);
                GetComponent<Rigidbody>().AddTorque(player.transform.forward * rollMagnitude, ForceMode.VelocityChange);
                fuel += ACCEL_FUEL_STEP;
            }
            if (isSpeedingUpCCW && isRollReady)
            {
                rollMagnitude = (float)-RollControlPatcher.config.ScubaRollSpeed / 100f * SCALING_FACTOR * (fuel / MAX_FUEL);
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
            float scalar = 1;
            if(GameInput.IsPrimaryDeviceGamepad())
            {
                scalar = RollControlPatcher.config.ScubaPadSensitivity;
            }
            else
            {
                scalar = RollControlPatcher.config.ScubaMouseSensitivity;
            }
            player.rigidBody.AddTorque(scalar / 100f * SCALING_FACTOR * player.transform.up * offset.x, ForceMode.VelocityChange);
            player.rigidBody.AddTorque(scalar / 100f * SCALING_FACTOR * player.transform.right * -offset.y, ForceMode.VelocityChange);

        }
        public void SetupScubaRoll()
        {
            bool portUp = Input.GetKeyUp(RollControlPatcher.config.RollPortKey);
            bool portHeld = Input.GetKey(RollControlPatcher.config.RollPortKey);
            bool portDown = Input.GetKeyDown(RollControlPatcher.config.RollPortKey);

            bool starUp = Input.GetKeyUp(RollControlPatcher.config.RollStarboardKey);
            bool starHeld = Input.GetKey(RollControlPatcher.config.RollStarboardKey);
            bool starDown = Input.GetKeyDown(RollControlPatcher.config.RollStarboardKey);

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
        public static void MaybeSetLocalEuler(Transform camTrans, Vector3 input)
        {
            if(swimstate == SwimState.RunFall || !isRollEnabled || Builder.isPlacing)
            {
                camTrans.localEulerAngles = input;
            }
        }

        public void OnCollisionEnter(Collision col)
        {
            if (!IsActuallyScubaRolling || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            player.rigidBody.freezeRotation = true;
        }
        public void OnCollisionStay(Collision col)
        {
            if (!IsActuallyScubaRolling || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            if (col.impulse == Vector3.zero)
            {
                player.rigidBody.freezeRotation = false;
            }
            else
            {
                player.rigidBody.freezeRotation = true;
            }
        }
        public void OnCollisionExit(Collision col)
        {
            if (!IsActuallyScubaRolling || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
            player.rigidBody.freezeRotation = false;
        }
    }
}
