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
    public enum CameraType
    {
        Player,
        Vehicle,
        Cyclops
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
        public CameraType camType;
        internal bool isScenicPiloting = false;
        internal bool allowLookDelta = true;
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

        #region unity_signals
        public void Start()
        {
            thirpyCamRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thirpyCamRoot.name = "ThirpyCamRoot";
            Component.DestroyImmediate(thirpyCamRoot.GetComponent<Renderer>());
            Component.DestroyImmediate(thirpyCamRoot.GetComponent<Collider>());
            thirpyCamRoot.transform.SetParent(Player.main.transform);
            PerVehicleConfig.Load();
        }
        public void Update()
        {
            if (!UWE.FreezeTime.HasFreezers())
            {
                UpdateCameraType();
                ComputeDerivedInputs(CollectInputs());
                UpdateThirpyState();
                UpdatePlayerHead();
                MoveCamera();
            }
            if (mode == ThirpyMode.Thirpy || mode == ThirpyMode.Scenic)
            {
                Player.main.SetHeadVisible(true);
                Player.main.SetScubaMaskActive(false);
            }
            if (Player.main.cinematicModeActive && Player.main.mode != Player.Mode.Piloting)
            {
                Player.main.SetHeadVisible(false);
            }
        }
        #endregion

        #region input_controls
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
        public ThirpyMode HandleScenicInput()
        {
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    isScenicPiloting = false;
                    return ThirpyMode.Scenic;
                case ThirpyMode.Scenic:
                    if (camType == CameraType.Cyclops)
                    {
                        if (!isScenicPiloting)
                        {
                            PerVehicleConfig.Save();
                        }
                        isScenicPiloting = !isScenicPiloting;
                        return ThirpyMode.Scenic;
                    }
                    else
                    {
                        isScenicPiloting = false;
                        PerVehicleConfig.Save();
                        if (camType != CameraType.Player)
                        {
                            isScenicPiloting = true;
                        }
                        return ThirpyMode.Scenic;
                    }
                case ThirpyMode.Thirpy:
                    isScenicPiloting = false;
                    return ThirpyMode.Scenic;
            }
            return ThirpyMode.Nothing;
        }
        public ThirpyMode HandleThirpyInput()
        {
            isScenicPiloting = false;
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    if (camType == CameraType.Cyclops)
                    {
                        isScenicPiloting = true;
                        return ThirpyMode.Scenic;
                    }
                    else
                    {
                        return ThirpyMode.Thirpy;
                    }
                case ThirpyMode.Scenic:
                    if (camType == CameraType.Cyclops)
                    {
                        return ThirpyMode.Nothing;
                    }
                    else
                    {
                        PerVehicleConfig.Save();
                        return ThirpyMode.Thirpy;
                    }
                case ThirpyMode.Thirpy:
                    return ThirpyMode.Nothing;
            }
            return ThirpyMode.Nothing;
        }
        #endregion

        #region thirpy_state
        private void UpdateCameraType()
        {
            CameraType newCamType = GetCameraType();
            if(newCamType != camType)
            {
                if (mode == ThirpyMode.Scenic)
                {
                    mode = ThirpyMode.Thirpy;
                }
                switch (newCamType)
                {
                    case CameraType.Player:
                        SetThirpyCamParent(Player.main.transform);
                        isScenicPiloting = false;
                        break;
                    case CameraType.Vehicle:
                        SetThirpyCamParent(Player.main.currentMountedVehicle.transform);
                        break;
                    case CameraType.Cyclops:
                        SetThirpyCamParent(Player.main.currentSub.transform);
                        break;
                }
                OnThirpyModeChanged();
                Pitch = PerVehicleConfig.GetPitch();
            }
            camType = newCamType;
        }
        private void UpdatePlayerHead()
        {
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    Player.main.SetHeadVisible(false);
                    Player.main.SetScubaMaskActive(Player.main.motorMode == Player.MotorMode.Dive || Player.main.motorMode == Player.MotorMode.Seaglide);
                    break;
                case ThirpyMode.Scenic:
                    Player.main.SetHeadVisible(true);
                    Player.main.SetScubaMaskActive(false);
                    break;
                case ThirpyMode.Thirpy:
                    Player.main.SetHeadVisible(true);
                    Player.main.SetScubaMaskActive(false);
                    break;
            }
        }
        private CameraType GetCameraType()
        {
            bool isInCyclops = Player.main.currentSub != null && Player.main.currentSub.name.ToLower().Contains("cyclops");
            if (isInCyclops && Player.main.mode == Player.Mode.Piloting)
            {
                Player.main.currentSub.GetComponent<FreezeRigidbodyWhenFar>().freezeDist = 150f;
                return CameraType.Cyclops;
            }
            else if (Player.main.currentMountedVehicle != null && Player.main.mode == Player.Mode.LockedPiloting)
            {
                return CameraType.Vehicle;
            }
            else
            {
                return CameraType.Player;
            }
        }
        public void UpdateThirpyState()
        {
            ThirpyMode newMode = mode;
            if (timeActivationInputsHeld == 0 && lastTimeActivationInputsHeld > 0)
            {
                if (lastTimeActivationInputsHeld > 0.25f)
                {
                    newMode = HandleScenicInput();
                }
                else
                {
                    newMode = HandleThirpyInput();
                }
            }
            if(mode != newMode)
            {
                mode = newMode;
                OnThirpyModeChanged();
            }
        }
        private void OnThirpyModeChanged()
        {
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    SetPlayerCamParent(PlayerCamPivot);
                    break;
                case ThirpyMode.Scenic:
                    SetPlayerCamParent(thirpyCamRoot.transform);
                    break;
                case ThirpyMode.Thirpy:
                    SetPlayerCamParent(thirpyCamRoot.transform);
                    break;
            }
        }
        #endregion

        #region camera_movement
        public void MoveCamera()
        {
            switch (mode)
            {
                case ThirpyMode.Nothing:
                    Player.main.GetComponent<PlayerController>().inputEnabled = MainCameraControl.main.enabled;
                    break;
                case ThirpyMode.Scenic:
                    HandleScenicMode();
                    break;
                case ThirpyMode.Thirpy:
                    switch (camType)
                    {
                        case CameraType.Player:
                            MovePlayerCamera();
                            break;
                        case CameraType.Vehicle:
                            MoveVehicleCamera();
                            break;
                        case CameraType.Cyclops:
                            MoveCyclopsCamera();
                            break;
                    }
                    break;
            }
        }
        private void MovePlayerCamera()
        {
            Player.main.GetComponent<PlayerController>().inputEnabled = true;
            thirpyCamRoot.transform.localPosition = SetupThirpyCamAndGetRawLocation();
            PlayerCam.transform.LookAt(Player.main.transform);
        }
        private void MoveVehicleCamera()
        {
            Player.main.GetComponent<PlayerController>().inputEnabled = true;
            Vector3 rawLocation = SetupThirpyCamAndGetRawLocation();
            Vector3 finalLocation = new Vector3(
                0,
                rawLocation.y,
                rawLocation.z
                );
            thirpyCamRoot.transform.localPosition = finalLocation;
            thirpyCamRoot.transform.LookAt(thirpyCamRoot.transform.parent);
        }
        private void MoveCyclopsCamera()
        {
            Player.main.GetComponent<PlayerController>().inputEnabled = false;
            Vector3 rawLocation = SetupThirpyCamAndGetRawLocation();
            Vector3 finalLocation = new Vector3(
                0,
                rawLocation.y,
                rawLocation.z
                );
            thirpyCamRoot.transform.localPosition = finalLocation;
            thirpyCamRoot.transform.LookAt(thirpyCamRoot.transform.parent);
        }
        private void HandleScenicMode()
        {
            Player.main.GetComponent<PlayerController>().inputEnabled = false;
            allowLookDelta = true;
            Vector2 lookVector = GameInput.GetLookDelta();
            if (isScenicPiloting)
            {
                allowLookDelta = false;
                AvatarInputHandler.main.gameObject.SetActive(true);
            }
            else
            {
                AvatarInputHandler.main.gameObject.SetActive(false);
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
                PerVehicleConfig.UpdatePitch(thirpyCamRoot.transform.localEulerAngles.x * Mathf.Deg2Rad);
            }
            thirpyCamRoot.transform.localEulerAngles = new Vector3(
                thirpyCamRoot.transform.localEulerAngles.x - lookVector.y,
                thirpyCamRoot.transform.localEulerAngles.y + lookVector.x,
                thirpyCamRoot.transform.localEulerAngles.z
                );
            thirpyCamRoot.transform.localPosition = Vector3.zero;
            Vector3 desiredWorldPosition = thirpyCamRoot.transform.parent.position - thirpyCamRoot.transform.forward * PerVehicleConfig.GetDistance();
            thirpyCamRoot.transform.position = desiredWorldPosition;
            thirpyCamRoot.transform.LookAt(thirpyCamRoot.transform.parent);
            PlayerCam.Find("camOffset/pdaCamPivot").transform.localEulerAngles = Vector3.zero;
        }
        private Vector3 SetupThirpyCamAndGetRawLocation()
        {
            AvatarInputHandler.main.gameObject.SetActive(true);
            thirpyCamRoot.transform.localPosition = Vector3.zero;
            thirpyCamRoot.transform.localRotation = Quaternion.identity;
            PlayerCam.transform.Find("camOffset").localEulerAngles = new Vector3(
                0f,
                PlayerCam.transform.Find("camOffset").localEulerAngles.y,
                0f
                );
            if (camType == CameraType.Player)
            {
                Vector2 lookVector2 = GameInput.GetLookDelta();
                Pitch -= lookVector2.y * pitchFactor;
            }
            else
            {
                Pitch = PerVehicleConfig.GetPitch();
            }
            Transform body;
            if (camType == CameraType.Player)
            {
                body = MainCameraControl.main.viewModel;
            }
            else
            {
                body = thirpyCamRoot.transform.parent;
            }
            Vector3 desiredWorldPosition2 = body.position;
            if (camType == CameraType.Cyclops)
            {
                // cyclops is backwards ?!
                desiredWorldPosition2 += body.forward * PerVehicleConfig.GetDistance() * Mathf.Cos(Pitch);
            }
            else
            {
                desiredWorldPosition2 += -body.forward * PerVehicleConfig.GetDistance() * Mathf.Cos(Pitch);
            }
            desiredWorldPosition2 += body.up * PerVehicleConfig.GetDistance() * Mathf.Sin(Pitch);
            Vector3 rawLocation = thirpyCamRoot.transform.parent.InverseTransformPoint(desiredWorldPosition2);
            return rawLocation;
        }
        #endregion

        #region utils
        public void SetPlayerCamParent(Transform parent)
        {
            PlayerCam.SetParent(parent);
            PlayerCam.localPosition = Vector3.zero;
            PlayerCam.localRotation = Quaternion.identity;
        }
        public void SetThirpyCamParent(Transform parent)
        {
            thirpyCamRoot.transform.SetParent(parent);
            thirpyCamRoot.transform.localPosition = Vector3.zero;
            thirpyCamRoot.transform.localRotation = Quaternion.identity;
        }
        #endregion
    }
}
