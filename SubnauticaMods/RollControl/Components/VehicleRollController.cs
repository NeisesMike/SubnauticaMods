using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RollControl
{
    public class VehicleRollController : MonoBehaviour
    {
        public Vehicle myVehicle;
        private bool isRollEnabled = RollControlPatcher.config.IsVehicleRollDefaultEnabled;

        public bool IsPlayerInThisVehicle
        {
            get
            {
                if(RollControlPatcher.ThereIsVehicleFramework)
                {
                    var vehicleTypesType = Type.GetType("VehicleFramework.VehicleTypes, VehicleFramework", false, false);
                    System.Reflection.PropertyInfo mountedDroneProperty;
                    Type vehicleTypesDroneType;
                    vehicleTypesDroneType = vehicleTypesType.GetNestedType("Drone");
                    System.Object mountedDrone = null;
                    if (vehicleTypesType != null && vehicleTypesDroneType != null)
                    {
                        mountedDroneProperty = vehicleTypesDroneType.GetProperty("mountedDrone");
                        mountedDrone = mountedDroneProperty.GetValue(null);
                    }
                    return Player.main.currentMountedVehicle == myVehicle
                        || (mountedDrone != null && (object)mountedDrone == myVehicle);
                }
                else
                {
                    return Player.main.currentMountedVehicle == myVehicle;
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(RollControlPatcher.config.ToggleRollKey) &&
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
            bool isUnderwater = myVehicle.transform.position.y < Ocean.GetOceanLevel() && myVehicle.transform.position.y < myVehicle.worldForces.waterDepth && !myVehicle.precursorOutOfWater;

            if (isRollEnabled &&
                isUnderwater &&
                IsPlayerInThisVehicle &&
                Player.main.mode == Player.Mode.LockedPiloting &&
                AvatarInputHandler.main.IsEnabled() &&
                myVehicle as Exosuit == null
                )
            {
                SubmarineRoll();
            }
        }

        public void SubmarineRoll()
        {
            if (Input.GetKey(RollControlPatcher.config.RollPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)RollControlPatcher.config.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
            if (Input.GetKey(RollControlPatcher.config.RollStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)-RollControlPatcher.config.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
        }
    }
}
