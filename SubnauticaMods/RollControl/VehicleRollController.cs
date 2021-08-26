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
            if (RollControlPatcher.Config.SubRoll && Player.main.currentMountedVehicle == myVehicle)
            {
                SubmarineRoll();
            }
        }

        public void SubmarineRoll()
        {
            if (Input.GetKey(RollControlPatcher.Config.RollPortKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)RollControlPatcher.Config.SubmarineRollSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(RollControlPatcher.Config.RollStarboardKey))
            {
                myVehicle.useRigidbody.AddTorque(myVehicle.transform.forward * (float)-RollControlPatcher.Config.SubmarineRollSpeed, ForceMode.VelocityChange);
            }
        }
    }
}
