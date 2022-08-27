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
        private bool isRollEnabled = RollControlPatcher.Config.IsVehicleRollDefaultEnabled;

        public void Update()
        {
            if (Input.GetKeyDown(RollControlPatcher.Config.ToggleRollKey) &&
                Player.main.currentMountedVehicle == myVehicle &&   
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
            Exosuit maybeExosuit = myVehicle as Exosuit;
            bool isUnderwater = myVehicle.transform.position.y < Ocean.main.GetOceanLevel() && myVehicle.transform.position.y < myVehicle.worldForces.waterDepth && !myVehicle.precursorOutOfWater;

            if (isRollEnabled &&
                isUnderwater && 
                Player.main.currentMountedVehicle == myVehicle &&
                Player.main.mode == Player.Mode.LockedPiloting &&
                AvatarInputHandler.main.IsEnabled() &&
                maybeExosuit == null
                )
            {
                SubmarineRoll();
            }
        }

        public void SubmarineRoll()
        {
            if (Input.GetKey(RollControlPatcher.Config.RollPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)RollControlPatcher.Config.SubmarineRollSpeed / 100f, ForceMode.VelocityChange);
            }
            if (Input.GetKey(RollControlPatcher.Config.RollStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)-RollControlPatcher.Config.SubmarineRollSpeed / 100f, ForceMode.VelocityChange);
            }
        }
    }
}
