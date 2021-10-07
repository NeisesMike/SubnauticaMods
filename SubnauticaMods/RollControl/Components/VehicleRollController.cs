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
        private bool isRollEnabled = true;

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
            if (isRollEnabled && 
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
