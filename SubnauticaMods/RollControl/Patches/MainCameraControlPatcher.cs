using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Collections;
using UWE;
using System.Reflection.Emit;

namespace RollControl
{
    [HarmonyPatch(typeof(MainCameraControl))]
    [HarmonyPatch(nameof(MainCameraControl.OnUpdate))]
    class MainCameraControlUpdatePatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int number_of_extra_instructions = 0;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newCodes = new List<CodeInstruction>(codes.Count + number_of_extra_instructions);
            CodeInstruction myNOP = new CodeInstruction(OpCodes.Nop);
            for (int i = 0; i < codes.Count + number_of_extra_instructions; i++)
            {
                newCodes.Add(myNOP);
            }
            for (int i = 0; i < codes.Count; i++)
            {
                if (
                       codes[i].opcode == OpCodes.Callvirt
                    && codes[i].operand.ToString().Contains("set_localEulerAngles")
                   )
                { 
                    codes[i] = CodeInstruction.Call(typeof(ScubaRollController), nameof(ScubaRollController.MaybeSetLocalEuler));
                }
                newCodes[i] = codes[i];
            }
            return newCodes.AsEnumerable();
        }
    }
}
