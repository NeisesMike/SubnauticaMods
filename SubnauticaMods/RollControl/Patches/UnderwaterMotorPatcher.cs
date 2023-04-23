using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection.Emit;

namespace RollControl.Patches
{
    [HarmonyPatch(typeof(UnderwaterMotor))]
    [HarmonyPatch(nameof(UnderwaterMotor.UpdateMove))]
    class UnderwaterMotorUpdateMovePatcher
    {
        /* This function handles input in a specific way.
         * Left/Right and F/B movement are rotated with the player. No problem.
         * But up/down is always WorldSpace up/down.
         * So that's not going to work.
         * In the OG function, the Y input gets removed,
         * then the remaining input is transformed into Subnautica movement,
         * then the Y is added back in with World Axes
         * So here we stop the Y from being removed,
         * and we stop it from being added back in.
         * That way, our up/down input gets transformed into "normal" Subnautica movement.
         */
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
            for (int i = 0; i < 3; i++)
            {
                newCodes[i] = codes[i];
            }
            for (int i = 3; i < codes.Count; i++)
            {

                if (
                    codes[i - 3].opcode == OpCodes.Call
                    && codes[i - 3].operand.ToString().Contains("Min")
                    && codes[i - 2].opcode == OpCodes.Ldloca_S
                    && codes[i - 1].opcode == OpCodes.Ldc_R4
                    && codes[i].opcode == OpCodes.Stfld
                   )
                {
                    // This bit stops our Y from being cancelled out.
                    newCodes[i - 2].opcode = OpCodes.Nop;
                    newCodes[i - 1].opcode = OpCodes.Nop;
                    codes[i].opcode = OpCodes.Nop;
                }

                if (
                    codes[i - 2].opcode == OpCodes.Ldind_R4
                    && codes[i - 1].opcode == OpCodes.Ldloc_3
                    && codes[i].opcode == OpCodes.Add
                   )
                {
                    // This stops our old Y from being added back in
                    newCodes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0f);
                }




                newCodes[i] = codes[i];

            }
            return newCodes.AsEnumerable();
        }
    }
}
