using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollControl
{
    class ControllerHandler
    {
        /*
        grabAxisInputs();
        if(yawAxis > 0 )
        {
        }
        */

        public string yawPortAxis;
        public string yawStarAxis;
        public string rollPortAxis;
        public string rollStarAxis;
        public string upThrustAxis;
        public string downThrustAxis;

        public float yawPort;
        public float yawStar;
        public float rollPort;
        public float rollStar;
        public float upThrust;
        public float downThrust;

        public ControllerHandler()
        {
            rollPortAxis = "ControllerAxis4";
            rollPortAxis = "ControllerAxis4";
        }

        public grabAxisInputs()
        {
            UnityEngine.Input.GetAxis(yawPortAxis);
        }
    }
}
