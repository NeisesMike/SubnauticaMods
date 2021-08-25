using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RollControl
{
    public class SeamothRollController : MonoBehaviour
    {
        public SeaMoth mySeamoth;
        private bool wasRollOn = false;

        public void Start()
        {
        }

        public void Update()
        {
            if (RollControlPatcher.isSeamothRollOn && !wasRollOn)
            {
                mySeamoth.stabilizeRoll = false;
                wasRollOn = true;
            }
            else if (!RollControlPatcher.isSeamothRollOn && wasRollOn)
            {
                mySeamoth.stabilizeRoll = true;
                wasRollOn = false;
                return;
            }
        }

        public void FixedUpdate()
        {
            if (RollControlPatcher.isSeamothRollOn && Player.main.currentMountedVehicle == mySeamoth)
            {
                SeamothRoll();
            }
        }

        public void SeamothRoll()
        {
            if (Input.GetKey(RollControlPatcher.Config.RollPortKey))
            {
                mySeamoth.useRigidbody.AddTorque(mySeamoth.transform.forward * (float)RollControlPatcher.Config.SeamothRollSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(RollControlPatcher.Config.RollStarboardKey))
            {
                mySeamoth.useRigidbody.AddTorque(mySeamoth.transform.forward * (float)-RollControlPatcher.Config.SeamothRollSpeed, ForceMode.VelocityChange);
            }
        }
    }
}
