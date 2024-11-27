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
        public static float CalculateConfigDepth()
        {
            float result = 0;
            result += (MainPatcher.MyConfig.thousands * 1000f);
            result += (MainPatcher.MyConfig.hundreds * 100f);
            return result;
        }
        public static float GetVehicleVoidDepth(CrushDamage crush)
        {
            float voidDepthMeters = CalculateConfigDepth();
            Vehicle vehicle = crush.gameObject.GetComponent<Vehicle>();
            SubRoot subroot = crush.gameObject.GetComponent<SubRoot>();
            if (vehicle != null)
            {
                int numModules = vehicle.GetCurrentUpgrades().Where(x => x.Contains(VoidDepth.upgradeName)).Count();
                return numModules * voidDepthMeters;
            }
            else if (subroot != null)
            {
                int numModules = subroot.GetCurrentUpgrades().Where(x => x.Contains(VoidDepth.upgradeName)).Count();
                return numModules * voidDepthMeters;
            }
            else
            {
                return 0;
            }
        }
    }
}
