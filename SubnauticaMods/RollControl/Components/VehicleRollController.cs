using System;
using UnityEngine;

namespace RollControl
{
    public class VehicleRollController : MonoBehaviour
    {
        public Vehicle myVehicle;
        private bool isRollEnabled = MainPatcher.RCConfig.IsVehicleRollDefaultEnabled;

        public bool IsPlayerInThisVehicle
        {
            get
            {
                if (MainPatcher.ThereIsVehicleFramework)
                {
                    return Player.main.currentMountedVehicle == myVehicle
                        || (VehicleFramework.VehicleTypes.Drone.MountedDrone != null && VehicleFramework.VehicleTypes.Drone.MountedDrone == myVehicle);
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
            if (Input.GetKeyDown(MainPatcher.RCConfig.ToggleRollKey) &&
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
            if (Input.GetKey(MainPatcher.RCConfig.VehicleRollPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
            if (Input.GetKey(MainPatcher.RCConfig.VehicleRollStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)-MainPatcher.RCConfig.SubmarineRollSpeed / 100f * 4f, ForceMode.VelocityChange);
            }
        }
    }
}
