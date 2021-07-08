using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FreeLook
{
    public static class FreeLookManager
    {
        // these are used as ref parameters in a sigmoidal lerp called smooth-damp-angle
        private static float xVelocity = 0.0f;
        private static float yVelocity = 0.0f;
        const float smoothTime = 0.25f;

        //static Vector3 cameraOffsetTransformPosition = new Vector3(0, 0, 0);

        public static bool isFreeLooking = false;

        // this controls whether update will be used to "snap back" the cursor to center
        public static bool resetCameraFlag = false;

        public static bool wasInVehicleLastFrame = false;
        public static bool isNewlyInVehicle = false;
        public static bool isNewlyOutVehicle = true;

        public static bool wasTriggerDownLastFrame = false;
        public static bool isTriggerNewlyDown = false;
        public static bool isTriggerNewlyUp = true;

        internal static void reset()
        {
            isFreeLooking = false;
            resetCameraFlag = false;

            wasInVehicleLastFrame = false;
            isNewlyInVehicle = false;
            isNewlyOutVehicle = true;

            wasTriggerDownLastFrame = false;
            isTriggerNewlyDown = false;
            isTriggerNewlyUp = true;
        }

        internal static void resetCameraRotation()
        {
            MainCameraControl mainCam = MainCameraControl.main;
            mainCam.rotationX = Mathf.SmoothDampAngle(mainCam.rotationX, 0f, ref xVelocity, smoothTime);
            mainCam.rotationY = Mathf.SmoothDampAngle(mainCam.rotationY, 0f, ref yVelocity, smoothTime);
            mainCam.camRotationX = mainCam.rotationX;
            mainCam.camRotationY = mainCam.rotationY;
            mainCam.cameraOffsetTransform.localEulerAngles = new Vector3(-mainCam.camRotationY, mainCam.camRotationX, 0);

            // if we're close enough to center, snap back and stop executing
            double threshold = 1;
            if (Mathf.Abs(mainCam.camRotationX) < threshold && Mathf.Abs(mainCam.camRotationY) < threshold)
            {
                mainCam.ResetCamera();
                cameraRelinquish();
                resetCameraFlag = false;
            }
        }

        internal static void moveCamera(Vehicle mySeamoth)
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            if (myLookDelta == Vector2.zero)
            {
                myLookDelta.x = Input.GetAxis("ControllerAxis4");
                myLookDelta.y = -Input.GetAxis("ControllerAxis5");
            }

            MainCameraControl mainCam = MainCameraControl.main;
            mainCam.rotationX += myLookDelta.x;
            mainCam.rotationY += myLookDelta.y;
            mainCam.rotationX = Mathf.Clamp(mainCam.rotationX, -100, 100);
            mainCam.rotationY = Mathf.Clamp(mainCam.rotationY, mainCam.minimumY, mainCam.maximumY);

            //mainCam.cameraUPTransform.localEulerAngles = new Vector3(Mathf.Min(0f, -mainCam.rotationY), 0f, 0f);

            mainCam.transform.localEulerAngles = new Vector3(-mainCam.rotationY, mainCam.rotationX, 0f);
        }

        internal static void cameraRelinquish()
        {
            var mainCamera = MainCameraControl.main;
            mainCamera.cinematicMode = false;
            //mainCamera.lookAroundMode = true;
            isFreeLooking = false;
            resetCameraFlag = false;
        }
    }
}
