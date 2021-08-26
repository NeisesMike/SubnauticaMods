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

        public void FixedUpdate()
        {
            Exosuit maybeExosuit = myVehicle as Exosuit;
            if (RollControlPatcher.Config.SubRoll && 
                Player.main.currentMountedVehicle == myVehicle && 
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
