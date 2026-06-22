using System;
using UnityEngine;

namespace RollControl
{
    public partial class VehicleRollController : MonoBehaviour
    {
        public Vehicle MyVehicle => gameObject.GetComponent<Vehicle>();
        public SeaTruckMotor MySeaTruck => gameObject.GetComponent<SeaTruckMotor>();
        internal bool isRollEnabled = MainPatcher.RCConfig.IsVehicleRollDefaultEnabled;

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
                    if (MySeaTruck.IsPiloted())
                    {
                        return true;
                    }
                    if (MyVehicle != null && MyVehicle == Player.main.currentMountedVehicle)
                    {
                        return true;
                    }
                    if (mountedDrone != null && mountedDrone == (object)MyVehicle)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (MySeaTruck.IsPiloted())
                    {
                        return true;
                    }
                    if (MyVehicle != null && MyVehicle == Player.main.currentMountedVehicle)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        private bool IsAboveWater
        {
            get
            {
                if(MyVehicle != null)
                {
                    return MyVehicle.worldForces.IsAboveWater();
                }
                else if(MySeaTruck != null)
                {
                    return MySeaTruck.transform.position.y >= 0f;

                }
                else
                {
                    throw new Exception("VehicleRollController was added to a non-vehicle component! Dying!");
                }
            }
        }

        public bool IsActuallyRolling
        {
            get
            {
                return isRollEnabled &&
                !IsAboveWater &&
                IsPlayerInThisVehicle &&
                Player.main.mode == Player.Mode.LockedPiloting &&
                AvatarInputHandler.main.IsEnabled() &&
                MyVehicle as Exosuit == null;
            }
        }

        public void Update()
        {
            if (MyVehicle != null && MyVehicle is Exosuit)
            {
                return;
            }
            if (Input.GetKeyDown(MainPatcher.RCConfig.ToggleRollKey) &&
                IsPlayerInThisVehicle &&
                AvatarInputHandler.main.IsEnabled())
            {
                isRollEnabled = !isRollEnabled;
                if (MyVehicle != null)
                {
                    MyVehicle.stabilizeRoll = !isRollEnabled;
                }
            }
        }

        public void FixedUpdate()
        {

            if (IsActuallyRolling)
            {

                if (MyVehicle != null)
                {
                    SubmarineRoll(MyVehicle.useRigidbody);
                }
                else if (MySeaTruck != null)
                {
                    SubmarineRoll(MySeaTruck.useRigidbody);

                }
                else
                {
                    throw new Exception("VehicleRollController was added to a non-vehicle component! Dying!");
                }
            }
        }

        public void SubmarineRoll(Rigidbody target)
        {
            if (Input.GetKey(MainPatcher.RCConfig.RollPortKey))
            {
                target.AddTorque(target.transform.forward * (float)MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
            if (Input.GetKey(MainPatcher.RCConfig.RollStarboardKey))
            {
                target.AddTorque(target.transform.forward * (float)-MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
        }
    }
}
