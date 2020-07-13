using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Harmony;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;

namespace RollControl
{
    [HarmonyPatch(typeof(GameInput))]
    [HarmonyPatch("Initialise")]

    public class GameInputPatcher
    {

        /*ref PilotingChair chair,*/
        [HarmonyPrefix]
        public static bool Prefix(GameInput __instance)
        {
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(GameInput __instance)
        {
		// rotations
		GameInput.AddAxisInput("Pitch", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		GameInput.AddAxisInput("Roll", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		GameInput.AddAxisInput("Yaw", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);

		// translations
		GameInput.AddAxisInput("Vertical Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		GameInput.AddAxisInput("Forward Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		GameInput.AddAxisInput("Sideways Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
        }
    }
}
