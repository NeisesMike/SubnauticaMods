﻿using System;
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
            PerVehicleConfig.Load();
        }
        public void Update()
        {
            if (!UWE.FreezeTime.HasFreezers())
            {
                MoveCamera(SetThirpyState(ComputeDerivedInputs(CollectInputs())));
            }
            if(mode == ThirpyMode.Thirpy || mode == ThirpyMode.Scenic)
            {
                Player.main.SetHeadVisible(true);
                Player.main.SetScubaMaskActive(false);
            }
            if(Player.main.cinematicModeActive && Player.main.mode != Player.Mode.Piloting)
            {
                Player.main.SetHeadVisible(false);
            }
        }
        public void MoveCamera(ThirpyInputs inputs)
        {
            switch(mode)
            {
                case ThirpyMode.Nothing:
                    Player.main.GetComponent<PlayerController>().inputEnabled = MainCameraControl.main.enabled;
                    break;
                case ThirpyMode.Thirpy:
                    AvatarInputHandler.main.gameObject.SetActive(true);
                    if(Player.main.GetMode() == Player.Mode.Piloting && Player.main.GetCurrentSub().name.ToLower().Contains("cyclops"))
                    {
                        Player.main.GetComponent<PlayerController>().inputEnabled = false;
                    }
                    else
                    {
                        Player.main.GetComponent<PlayerController>().inputEnabled = true;
                    }
                    thirpyCamRoot.transform.localPosition = Vector3.zero;
                    thirpyCamRoot.transform.localRotation = Quaternion.identity;
                    Transform body = MainCameraControl.main.viewModel;
                    PlayerCam.transform.Find("camOffset").localEulerAngles = new Vector3(
                        0f,
                        PlayerCam.transform.Find("camOffset").localEulerAngles.y,
                        0f
                        );

                    if (Player.main.GetVehicle() == null || Player.main.mode == Player.Mode.Normal)
                    {
                        Vector2 lookVector2 = GameInput.GetLookDelta();
                        Pitch -= lookVector2.y * pitchFactor;
                    }
                    else
                    {
                        Pitch = 0.3f;
                    }
                    Vector3 desiredWorldPosition2 = body.position;
                    desiredWorldPosition2 += -body.forward * PerVehicleConfig.GetDistance() * Mathf.Cos(Pitch);
                    desiredWorldPosition2 += body.up * PerVehicleConfig.GetDistance() * Mathf.Sin(Pitch);
                    thirpyCamRoot.transform.localPosition = thirpyCamRoot.transform.parent.InverseTransformPoint(desiredWorldPosition2);
                    PlayerCam.transform.LookAt(Player.main.transform);
                    break;
                case ThirpyMode.Scenic:
                    Player.main.GetComponent<PlayerController>().inputEnabled = false;
                    AvatarInputHandler.main.gameObject.SetActive(false);
                    Vector2 lookVector = GameInput.GetLookDelta();
                    thirpyCamRoot.transform.localEulerAngles = new Vector3(
                        thirpyCamRoot.transform.localEulerAngles.x - lookVector.y,
                        thirpyCamRoot.transform.localEulerAngles.y + lookVector.x,
                        thirpyCamRoot.transform.localEulerAngles.z
                        );
                    thirpyCamRoot.transform.localPosition = Vector3.zero;
                    Vector3 desiredWorldPosition = thirpyCamRoot.transform.position - thirpyCamRoot.transform.forward * PerVehicleConfig.GetDistance();
                    thirpyCamRoot.transform.localPosition = thirpyCamRoot.transform.parent.InverseTransformPoint(desiredWorldPosition);
                    float zoomIn = GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
                    float zoomInFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
                    float zoomOut = GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
                    float zoomOutFine = GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);

                    float currentZoom = PerVehicleConfig.GetDistance();
                    currentZoom -= zoomIn * 0.25f;
                    currentZoom -= zoomInFine * 0.01f;
                    currentZoom += zoomOut * 0.25f;
                    currentZoom += zoomOutFine * 0.01f;
                    PerVehicleConfig.UpdateDistance(currentZoom);
                    break;
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
                    PerVehicleConfig.Save();
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
                    PerVehicleConfig.Save();
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
        public void UpdateCyclopsFreeze()
        {
            if(Player.main.currentSub.name.ToLower().Contains("cyclops"))
            {
                Player.main.currentSub.GetComponent<FreezeRigidbodyWhenFar>().freezeDist = 150f;
            }
        }
    }
}
