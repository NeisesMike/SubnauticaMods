using System;
using UnityEngine;

namespace RollControl
{
    public partial class VehicleRollController : MonoBehaviour
    {
        public Vehicle myVehicle;
        private bool isRollEnabled = MainPatcher.RCConfig.IsVehicleRollDefaultEnabled;

        public bool IsPlayerInThisVehicle
        {
            get
            {
                if (MainPatcher.ThereIsVehicleFramework)
                {
                    var vehicleTypesDroneType = Type.GetType("VehicleFramework.VehicleTypes.Drone, VehicleFramework", false, false);
                    if (vehicleTypesDroneType == null)
                    {
                        Logger.Log("vehicletypestype drone was null");
                        return false;
                    }
                    System.Object mountedDrone = null;
                    if (vehicleTypesDroneType != null)
                    {
                        var memberInfo = vehicleTypesDroneType.GetMember("MountedDrone")?.FirstOrFallback(null);
                        if (memberInfo == null)
                        {
                            Logger.Log("Roll Control had an issue with finding Drone.MountedDrone...");
                            return false;
                        }
                        if (memberInfo is System.Reflection.PropertyInfo propertyInfo)
                        {
                            mountedDrone = propertyInfo.GetValue(null);
                        }
                        else if (memberInfo is System.Reflection.FieldInfo fieldInfo)
                        {
                            mountedDrone = fieldInfo.GetValue(null);
                        }
                    }
                    return Player.main.currentMountedVehicle == myVehicle
                        || (mountedDrone != null && mountedDrone == (object)myVehicle);
                }
                else
                {
                    return Player.main.currentMountedVehicle == myVehicle;
                }
            }
        }

        public bool IsActuallyRolling
        {
            get
            {
                bool isUnderwater = myVehicle.transform.position.y < Ocean.GetOceanLevel() && myVehicle.transform.position.y < myVehicle.worldForces.waterDepth && !myVehicle.precursorOutOfWater;
                return isRollEnabled &&
                isUnderwater &&
                IsPlayerInThisVehicle &&
                Player.main.mode == Player.Mode.LockedPiloting &&
                AvatarInputHandler.main.IsEnabled() &&
                myVehicle as Exosuit == null;
            }
        }

        public void Update()
        {
            if (GameInput.GetButtonDown(MainPatcher.Instance.ToggleRollKey) &&
                IsPlayerInThisVehicle &&
                AvatarInputHandler.main.IsEnabled() &&
                (myVehicle as Exosuit) == null
                )
            {
                isRollEnabled = !isRollEnabled;
                myVehicle.stabilizeRoll = !isRollEnabled;
            }
        }

        public void FixedUpdate()
        {

            if (IsActuallyRolling)
            {
                SubmarineRoll();
            }
        }

        public void SubmarineRoll()
        {
            if (GameInput.GetButtonHeld(MainPatcher.Instance.RollPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
            if (GameInput.GetButtonHeld(MainPatcher.Instance.RollStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)-MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
        }
    }
}
