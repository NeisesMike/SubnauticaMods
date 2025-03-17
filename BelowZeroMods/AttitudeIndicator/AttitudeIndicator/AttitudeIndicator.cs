using UnityEngine;

namespace AttitudeIndicator
{
    internal class AttitudeIndicator : MonoBehaviour
    {
        private Transform Model => transform.Find("InstrumentModel");
        private Transform Globe => Model.Find("Globe");
        private Transform Ring => Model.Find("Ring");
        private Vehicle MyVehicle => GetComponentInParent<Vehicle>();
        private SubRoot MySub => GetComponentInParent<SubRoot>();
        private void Update()
        {
            if (UpdateEnabled())
            {
                Model.gameObject.SetActive(true);
                UpdatePosition();
                UpdateRotations();
            }
            else
            {
                Model.gameObject.SetActive(false);
            }
        }
        private bool UpdateEnabled()
        {
            VehicleFramework.ModVehicle mv = transform.parent.GetComponent<VehicleFramework.ModVehicle>();
            if (mv != null)
            {
                string mvName = mv.GetComponent<TechTag>().type.AsString();
                bool isMVEnabled = VehicleFramework.Admin.ExternalVehicleConfig<bool>.GetModVehicleConfig(mvName).GetValue(InstrumentConfig.enabledString);
                return isMVEnabled && Player.main.currentMountedVehicle == mv && mv.IsPlayerControlling();
            }
            else
            {
                switch (GetTechType())
                {
                    case TechType.Seamoth:
                        bool isSeamothEnabled = VehicleFramework.Admin.ExternalVehicleConfig<bool>.GetSeamothConfig().GetValue(InstrumentConfig.enabledString);
                        return isSeamothEnabled && Player.main.currentMountedVehicle == MyVehicle;
                    case TechType.Cyclops:
                        bool isCyclopsEnabled = VehicleFramework.Admin.ExternalVehicleConfig<bool>.GetCyclopsConfig().GetValue(InstrumentConfig.enabledString);
                        return isCyclopsEnabled && Player.main.currentSub == MySub && Player.main.GetMode() == Player.Mode.Piloting;
                    default:
                        return false;
                }
            }
        }
        private void UpdatePosition()
        {
            TechType myTT = GetTechType();
            float xPosition;
            float yPosition;
            float zPosition;
            float scale;
            VehicleFramework.ModVehicle mv = transform.parent.GetComponent<VehicleFramework.ModVehicle>();
            if (mv != null)
            {
                string mvName = mv.GetComponent<TechTag>().type.AsString();
                xPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(InstrumentConfig.xString);
                yPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(InstrumentConfig.yString);
                zPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(InstrumentConfig.zString);
                scale = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetModVehicleConfig(mvName).GetValue(InstrumentConfig.scaleString);
            }
            else
            {
                switch (myTT)
                {
                    case TechType.Seamoth:
                        xPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(InstrumentConfig.xString);
                        yPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(InstrumentConfig.yString);
                        zPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(InstrumentConfig.zString);
                        scale = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetSeamothConfig().GetValue(InstrumentConfig.scaleString);
                        break;
                    case TechType.Cyclops:
                        xPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(InstrumentConfig.xString);
                        yPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(InstrumentConfig.yString);
                        scale = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(InstrumentConfig.scaleString);
                        zPosition = VehicleFramework.Admin.ExternalVehicleConfig<float>.GetCyclopsConfig().GetValue(InstrumentConfig.zString);
                        break;
                    default:
                        xPosition = 0;
                        yPosition = 0;
                        zPosition = 0;
                        scale = 0;
                        break;
                }
            }

            transform.localScale = Vector3.one * scale;
            transform.position = MainCameraControl.main.transform.position
                + MainCameraControl.main.transform.forward * zPosition
                + MainCameraControl.main.transform.right * xPosition
                + MainCameraControl.main.transform.up * yPosition;
        }
        private void UpdateRotations()
        {
            float playerPitch = MainCameraControl.main.transform.eulerAngles.x;
            float playerRoll = MainCameraControl.main.transform.eulerAngles.z;

            gameObject.transform.LookAt(MainCamera.camera.transform, MainCamera.camera.transform.up);

            var ringLocalEulers = Ring.localEulerAngles;
            ringLocalEulers.z = playerRoll - 90;
            Ring.localEulerAngles = ringLocalEulers;

            var globeLocalEulers = Globe.localEulerAngles;
            globeLocalEulers.z = playerPitch - 90;
            Globe.localEulerAngles = globeLocalEulers;
        }
        private TechType GetTechType()
        {
            TechTag thisTechTag = transform.parent.GetComponent<TechTag>();
            if (thisTechTag != null)
            {
                return thisTechTag.type;
            }
            else
            {
                SubRoot thisSubRoot = transform.parent.GetComponent<SubRoot>();
                if (thisSubRoot != null && thisSubRoot.isCyclops)
                {
                    return TechType.Cyclops;
                }
            }
            return TechType.None;
        }
    }
}

