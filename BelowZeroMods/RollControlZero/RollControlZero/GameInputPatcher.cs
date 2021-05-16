using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;

namespace RollControlZero
{
    public class InputManager
    {
        //GameInput.AnalogAxis portRoll;
        /*
        string portRollAxis;
        string starboardRollAxis;

         public InputManager()
        {
            string portRollAxis = "ControllerAxis1";
            string starboardRollAxis = "ControllerAxis2";
        }
        */
    }

    [HarmonyPatch(typeof(GameInput))]
    [HarmonyPatch("Awake")]
    public class GameInputPatcher
    {
        public static InputManager myInputMan;

        [HarmonyPrefix]
        public static bool Prefix(GameInput __instance)
        {
            // initialize the roll manager
            myInputMan = new InputManager();

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(GameInput __instance)
        {
            /*
		    // rotations
		    GameInput.AddAxisInput("Pitch", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		    GameInput.AddAxisInput("Roll", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		    GameInput.AddAxisInput("Yaw", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);

		    // translations
		    GameInput.AddAxisInput("Vertical Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		    GameInput.AddAxisInput("Forward Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
		    GameInput.AddAxisInput("Sideways Thrust", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0f);
            */
        }
    }
}
