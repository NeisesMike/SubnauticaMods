using UnityEngine;

namespace FreeRead
{
    internal class FreeReadManager : MonoBehaviour
    {
        internal struct InputPair
        {
            internal bool lockCursor;
            internal bool avatarEnabled;
        }
        internal InputPair lastInputPair;
        internal bool isFreeReading = false;
        internal bool wasLockMovement = false;

        private void Update()
        {
            bool isAutoMovePressed = GameInput.GetButtonDown(GameInput.Button.AutoMove);
            bool isInputDisabled = !AvatarInputHandler.main.IsEnabled();
            bool isThisCurrentVehicle = GetComponent<Vehicle>() == Player.main.currentMountedVehicle;
            if (isAutoMovePressed && isInputDisabled && isThisCurrentVehicle && isFreeReading)
            {
                if (GameInput.AutoMove)
                {
                    GameInput.AutoMove = false;
                }
                else
                {
                    GameInput.AutoMove = true;
                }
            }
        }
    }
}
