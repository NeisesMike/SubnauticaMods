using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using UWE;
using System.Reflection.Emit;

namespace FreeRead
{
    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("Close")]
    class PDAClosePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            GameInput.SetAutoMove(FreeReadPatcher.isCruising);
            FreeReadPatcher.isCruising = false;
            FreeReadPatcher.isInPDA = false;
        }
    }

    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("Open")]
    class PDAOpenPatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            FreeReadPatcher.isInPDA = true;
            if (GameInput.GetAutoMove())
            {
               FreeReadPatcher.isCruising = true;
            }
        }
    }

    [HarmonyPatch(typeof(PDA))]
    [HarmonyPatch("ManagedUpdate")]
    class PDAManagedUpdatePatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newCodes = new List<CodeInstruction>(codes.Count+2);

            CodeInstruction myNOP = new CodeInstruction(OpCodes.Nop);
            for (int i=0; i<codes.Count+2; i++)
            {
                newCodes.Add(myNOP);
            }

            bool haveAddedOurLine = false;
            for (int i = 0; i < codes.Count; i++)
            {
				if (!haveAddedOurLine && codes[i].opcode == OpCodes.Ldsfld)
                {
                    CodeInstruction newInstr = CodeInstruction.LoadField(typeof(FreeReadOptions), "isAllowingPause");

                    newCodes[i] = codes[i];
                    newCodes[i + 1] = newInstr;
                    newCodes[i + 2].opcode = OpCodes.And;
					haveAddedOurLine = true;
                    continue;
				}
                if (haveAddedOurLine)
				{
					newCodes[i+2] = codes[i];
                }
                else
                {
					newCodes[i] = codes[i];
                }
            }
            return newCodes.AsEnumerable();
		}
	}
}
