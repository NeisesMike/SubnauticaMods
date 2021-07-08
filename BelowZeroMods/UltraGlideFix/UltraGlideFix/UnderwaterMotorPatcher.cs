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

namespace UltraGlideFix
{
    /*
     * AlterMaxSpeed is working correctly
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
    [HarmonyPatch(typeof(UnderwaterMotor))]
    [HarmonyPatch("UpdateMove")]
    class UnderwaterMotorUpdateMovePatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newCodes = new List<CodeInstruction>(codes.Count + 2);

            CodeInstruction myNOP = new CodeInstruction(OpCodes.Nop);
            for (int i = 0; i < codes.Count + 1; i++)
            {
                newCodes.Add(myNOP);
            }

            bool haveAddedOurLine1 = false;
            bool haveAddedOurLine2 = false;

            for (int i = 0; i < codes.Count; i++)
            {
                /*
                // add two instructions
                if (!(haveAddedOurLine1 && haveAddedOurLine2) && codes[i].operand != null)
                {
                    if (codes[i].operand.ToString().Contains("waterAcceleration"))
                    {
                        CodeInstruction newInstr = CodeInstruction.Call(typeof(Logger), "UpdateAcceleration");
                        newCodes[i] = codes[i];
                        newCodes[i + 1] = newInstr;
                        haveAddedOurLine1 = true;
                        continue;
                    }
                    if (codes[i].operand.ToString().Contains("AlterMaxSpeed"))
                    {
                        haveBeenToAlterMaxSpeed = true;
                        newCodes[i] = codes[i];
                        continue;
                    }
                    if (haveBeenToAlterMaxSpeed && codes[i].operand.ToString().Contains("Max"))
                    {
                        newCodes[i] = codes[i];
                        haveBeenToMax = true;
                        continue;
                    }
                    if (haveBeenToMax && codes[i].operand.ToString().Contains("get_magnitude"))
                    {
                        CodeInstruction newInstr = CodeInstruction.Call(typeof(Logger), "OutputFloat");
                        haveAddedOurLine2 = true;
                        newCodes[i + 1] = codes[i];
                        newCodes[i + 2] = newInstr;
                        continue;
                    }
                }
                */

                // add one instruction
                if (!haveAddedOurLine1 && codes[i].operand != null)
                {

                    if (codes[i].operand.ToString().Contains("waterAcceleration"))
                    {
                        CodeInstruction newInstr = CodeInstruction.Call(typeof(Logger), "UpdateAcceleration");
                        newCodes[i] = codes[i];
                        newCodes[i + 1] = newInstr;
                        haveAddedOurLine1 = true;
                        continue;
                    }

                    /*
                    if (codes[i].operand.ToString().Contains("set_drag"))
                    {
                        CodeInstruction newInstr = new CodeInstruction(OpCodes.Ldc_R4, 1f);
                        haveAddedOurLine = true;
                        newCodes[i - 1] = newInstr;
                        newCodes[i] = codes[i];
                        continue;
                    }

                    if (codes[i].operand.ToString().Contains("AlterMaxSpeed"))
                    {
                        haveBeenToAlterMaxSpeed = true;
                        newCodes[i] = codes[i];
                        continue;
                    }
                    if (haveBeenToAlterMaxSpeed && codes[i].operand.ToString().Contains("Max"))
                    {
                        newCodes[i] = codes[i];
                        haveBeenToMax = true;
                        continue;
                    }
                    if (haveBeenToMax && codes[i].operand.ToString().Contains("get_magnitude"))
                    {
                        CodeInstruction newInstr = CodeInstruction.Call(typeof(Logger), "OutputFloat");
                        haveAddedOurLine1 = true;
                        newCodes[i] = codes[i];
                        newCodes[i + 1] = newInstr;
                        continue;
                    }
                    */
                }

                if (haveAddedOurLine1 ^ haveAddedOurLine2)
                {
                    newCodes[i + 1] = codes[i];
                }
                else if (haveAddedOurLine1 && haveAddedOurLine2)
                {
                    newCodes[i + 2] = codes[i];
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
