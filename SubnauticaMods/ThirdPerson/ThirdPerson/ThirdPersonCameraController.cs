using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThirdPerson
{
    public struct ActivationInputs
    {
        public bool isUpHeld;
        public bool isDownHeld;
    }
    public struct MovementInputs
    {
        public float zoomIn;
        public float zoomOut;
        public float zoomInFine;
        public float zoomOutFine;
    }
    public enum ThirpyMode
    {
        Nothing,
        Scenic,
        Thirpy
    }
    public struct ThirpyInputs
    {
        public ActivationInputs active;
        public MovementInputs move;
    }
    public class ThirdPersonCameraController : MonoBehaviour
    {
        private float lastTimeActivationInputsHeld = 0f;
        private float timeActivationInputsHeld = 0f;
        internal ThirpyMode mode;
        private GameObject thirpyCamRoot = null;
        internal float thirpyZoom = 5.1f;
        internal float pitchFactor = 0.01f;
        private float _pitch = 0;
        private float Pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                _pitch = value;
                if(_pitch > 0.98f)
                {
                    _pitch = 0.98f;
                }
                if (_pitch < -0.98f)
                {
                    _pitch = -0.98f;
                }
            }
        }
        public void Start()
        {
            thirpyCamRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Component.DestroyImmediate(thirpyCamRoot.GetComponent<Renderer>());
            Component.DestroyImmediate(thirpyCamRoot.GetComponent<Collider>());
            thirpyCamRoot.transform.SetParent(Player.main.transform);
        }
        public void Update()
        {
            MoveCamera(SetThirpyState(ComputeDerivedInputs(CollectInputs())));
        }
        public void MoveCamera(ThirpyInputs inputs)
        {
            AvatarInputHandler.main.gameObject.SetActive(true);
            Player.main.GetComponent<PlayerController>().inputEnabled = mode != ThirpyMode.Scenic;
            if (mode == ThirpyMode.Scenic)
            {
                AvatarInputHandler.main.gameObject.SetActive(false);
                Vector2 lookVector = GameInput.GetLookDelta();
                thirpyCamRoot.transform.localEulerAngles = new Vector3(
                    thirpyCamRoot.transform.localEulerAngles.x - lookVector.y,
                    thirpyCamRoot.transform.localEulerAngles.y + lookVector.x,
                    thirpyCamRoot.transform.localEulerAngles.z
                    );
                thirpyCamRoot.transform.localPosition = Vector3.zero;
                Vector3 desiredWorldPosition = thirpyCamRoot.transform.position - thirpyCamRoot.transform.forward * thirpyZoom;
                thirpyCamRoot.transform.localPosition = thirpyCamRoot.transform.parent.InverseTransformPoint(desiredWorldPosition);
                float zoomIn = GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
                float zoomInFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
                float zoomOut = GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
                float zoomOutFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
                thirpyZoom -= zoomIn * 0.25f;
                thirpyZoom -= zoomInFine * 0.01f;
                thirpyZoom += zoomOut * 0.25f;
                thirpyZoom += zoomOutFine * 0.01f;
            }
            else if(mode == ThirpyMode.Thirpy)
            {
                thirpyCamRoot.transform.localPosition = Vector3.zero;
                thirpyCamRoot.transform.localRotation = Quaternion.identity;
                Transform body = MainCameraControl.main.viewModel;
                PlayerCam.transform.Find("camOffset").localEulerAngles = new Vector3(
                    0f,
                    PlayerCam.transform.Find("camOffset").localEulerAngles.y,
                    0f
                    );

                if(Player.main.GetVehicle() == null)
                {
                    Vector2 lookVector = GameInput.GetLookDelta();
                    Pitch -= lookVector.y * pitchFactor;
                }
                else
                {
                    Pitch = 0.3f;
                }
                Vector3 desiredWorldPosition = body.position;
                desiredWorldPosition += -body.forward * thirpyZoom * Mathf.Cos(Pitch);
                desiredWorldPosition += body.up * thirpyZoom * Mathf.Sin(Pitch);
                thirpyCamRoot.transform.localPosition = thirpyCamRoot.transform.parent.InverseTransformPoint(desiredWorldPosition);
                PlayerCam.transform.LookAt(Player.main.transform);
            }
        }
        public ThirpyInputs SetThirpyState(ThirpyInputs inputs)
        {
            if (timeActivationInputsHeld == 0 && lastTimeActivationInputsHeld > 0)
            {
                if (lastTimeActivationInputsHeld > 1)
                {
                    return HandleScenicInput(inputs);
                }
                else
                {
                    return HandleThirpyInput(inputs);
                }
            }
            return inputs;
        }
        public ThirpyInputs ComputeDerivedInputs(ThirpyInputs inputs)
        {
            lastTimeActivationInputsHeld = timeActivationInputsHeld;
            if (inputs.active.isDownHeld && inputs.active.isUpHeld)
            {
                timeActivationInputsHeld += Time.deltaTime;
            }
            else
            {
                timeActivationInputsHeld = 0f;
            }
            return inputs;
        }
        public ThirpyInputs CollectInputs()
        {
            ActivationInputs active = new ActivationInputs
            {
                isDownHeld = GameInput.GetButtonHeld(GameInput.Button.MoveDown),
                isUpHeld = GameInput.GetButtonHeld(GameInput.Button.MoveUp)
            };
            MovementInputs move = new MovementInputs
            {
                zoomIn = GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp),
                zoomOut = GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown),
                zoomInFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight),
                zoomOutFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft),
            };
            return new ThirpyInputs
            {
                active = active,
                move = move
            };
        }
        public ThirpyInputs HandleScenicInput(ThirpyInputs inputs)
        {
            switch(mode)
            {
                case ThirpyMode.Nothing:
                    ActivateThirpyCam(true);
                    mode = ThirpyMode.Scenic;
                    break;
                case ThirpyMode.Scenic:
                    ActivateThirpyCam(true);
                    mode = ThirpyMode.Thirpy;
                    break;
                case ThirpyMode.Thirpy:
                    ActivateThirpyCam(true);
                    mode = ThirpyMode.Scenic;
                    break;
            }
            return inputs;
        }
        public ThirpyInputs HandleThirpyInput(ThirpyInputs inputs)
        {
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    ActivateThirpyCam(true);
                    mode = ThirpyMode.Thirpy;
                    break;
                case ThirpyMode.Scenic:
                    ActivateThirpyCam(true);
                    mode = ThirpyMode.Thirpy;
                    break;
                case ThirpyMode.Thirpy:
                    ActivateThirpyCam(false);
                    mode = ThirpyMode.Nothing;
                    break;
            }
            return inputs;
        }
        public void ActivateThirpyCam(bool enabled)
        {
            Pitch = 0.3f;
            Player.main.SetHeadVisible(enabled);
            Player.main.SetScubaMaskActive(!enabled && (Player.main.motorMode == Player.MotorMode.Dive || Player.main.motorMode == Player.MotorMode.Seaglide));
            if (enabled)
            {
                MovePlayerCameraToTransform(thirpyCamRoot.transform);
            }
            else
            {
                MovePlayerCameraToTransform(PlayerCamPivot);
            }
        }

        private Transform playerCamActual = null;
        public Transform PlayerCam
        {
            get
            {
                if (playerCamActual == null)
                {
                    playerCamActual = PlayerCamPivot.Find("camRoot");
                }
                return playerCamActual;
            }
        }
        public Transform PlayerCamPivot => Player.main.transform.Find("camPivot");
        public void MovePlayerCameraToTransform(Transform destination)
        {
            PlayerCam.SetParent(destination);
            PlayerCam.localPosition = Vector3.zero;
            PlayerCam.localRotation = Quaternion.identity;
        }
    }
}
