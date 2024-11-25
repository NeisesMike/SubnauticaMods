using System;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;
using VehicleFramework;
using System.Linq;

namespace VoidDepth
{
    [HarmonyPatch(typeof(CrushDamage))]
    public class CrushDamagePatcher
    {
        [HarmonyPatch(nameof(CrushDamage.UpdateDepthClassification))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch ReturnValidCraftingPositionMatch = new CodeMatch(i => i.opcode == OpCodes.Call && i.operand.ToString().Contains("get_extraCrushDepth"));

            var newInstructions = new CodeMatcher(instructions)
                .MatchStartForward(ReturnValidCraftingPositionMatch)
                .Advance(2)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(Transpilers.EmitDelegate<Func<CrushDamage, float>>(GetVehicleVoidDepth))
                .Insert(new CodeInstruction(OpCodes.Add));

            return newInstructions.InstructionEnumeration();
        }
        public static float GetVehicleVoidDepth(CrushDamage crush)
        {
            const float voidDepthMeters = VoidDepth.crushAddition;
            Vehicle vehicle = crush.gameObject.GetComponent<Vehicle>();
            if(vehicle == null)
            {
                return 0;
            }
            int numModules = vehicle.GetCurrentUpgrades().Where(x => x.Contains(VoidDepth.upgradeName)).Count();
            return numModules * voidDepthMeters;
        }
    }
}
