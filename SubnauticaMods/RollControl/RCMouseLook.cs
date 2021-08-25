using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RollControl
{
    public class RCMouseLook : MonoBehaviour
    {
        private bool wasScubaRolling = false;

        public void Start()
        {
        }

        public void Update()
        {
            bool isScubaRolling = RollControlPatcher.isScubaRollOn && (GetComponent<Player>().motorMode == Player.MotorMode.Dive || GetComponent<Player>().motorMode == Player.MotorMode.Seaglide);
            if(isScubaRolling)
            {
                MainCameraControl.main.rotationX = 0;
                MainCameraControl.main.rotationY = 0;
                MainCameraControl.main.SetEnabled(false);
                Player.main.armsController.enabled = false;
                Player.main.rigidBody.angularDrag = 15;
                PhysicsMouseLook();
            }
            else if(!isScubaRolling)
            {
                Player.main.rigidBody.angularDrag = 4;
                MainCameraControl.main.SetEnabled(true);
                Player.main.armsController.enabled = true;
            }
        }

        public void PhysicsMouseLook()
        {
            Vector2 offset = GameInput.GetLookDelta();

            Player.main.rigidBody.AddTorque(0.3f * Player.main.transform.up * offset.x, ForceMode.VelocityChange);

            Player.main.rigidBody.AddTorque(0.3f * Player.main.transform.right * -offset.y, ForceMode.VelocityChange);


            MainCameraControl.main.transform.rotation = Player.main.transform.rotation;
            MainCameraControl.main.transform.localRotation = Quaternion.identity;
        }
    }
}
