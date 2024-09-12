using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace ThirdPerson
{

    [HarmonyPatch(typeof(GameInput))]
    public static class GameInputPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameInput.GetMoveDirection))]
        public static void GetMoveDirectionPostfix(GameInput __instance, ref Vector3 __result)
        {
            //don't allow vehicle movement if we're in scenic mode
            if (Player.main.GetComponent<ThirdPersonCameraController>().mode == ThirpyMode.Scenic)
            {
                if (Player.main.GetVehicle() != null)
                {
                    __result = Vector3.zero;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainCameraControl))]
    public static class MainCameraControlPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MainCameraControl.OnUpdate))]
        public static bool OnUpdatePrefix()
        {
            // don't update in scenic mode.
            return Player.main.GetComponent<ThirdPersonCameraController>().mode != ThirpyMode.Scenic;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainCameraControl.GetCameraBob))]
        public static void UpdateCamShakePostfix(MainCameraControl __instance, ref bool __result)
        {
            //don't bob unless we're in first person
            if (Player.main.GetComponent<ThirdPersonCameraController>().mode != ThirpyMode.Nothing)
            {
                __result = false;
            }
        }


        [HarmonyPatch(typeof(Targeting))]
        public static class TargetingPatcher
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return typeof(Targeting).GetMethod("GetTarget", new Type[] { typeof(float), typeof(GameObject).MakeByRefType(), typeof(float).MakeByRefType() });
            }

            [HarmonyPrefix]
            public static void GetTargetPrefix(Targeting __instance, ref float maxDistance)
            {
                if (Player.main.GetComponent<ThirdPersonCameraController>().mode == ThirpyMode.Thirpy)
                {
                    maxDistance += PerVehicleConfig.GetDistance();
                }
            }
        }
    }
}
