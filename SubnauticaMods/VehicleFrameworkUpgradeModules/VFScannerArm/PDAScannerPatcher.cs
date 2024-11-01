using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection.Emit;
using VehicleFramework;
using UnityEngine;

namespace VFScannerArm
{
    [HarmonyPatch(typeof(PDAScanner))]
    public static class PDAScannerPatcher
    {
        delegate bool MyDelegate(float maxDistance, out GameObject result, out float distance);

        [HarmonyPatch(nameof(PDAScanner.UpdateTarget))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch getTargetMatch = new CodeMatch(i => i.opcode == OpCodes.Call && i.operand.ToString().Contains("GetTarget"));

            var newInstructions = new CodeMatcher(instructions)
                .MatchStartForward(getTargetMatch)
                .SetInstruction(Transpilers.EmitDelegate<MyDelegate>(CallOtherGetTarget));

            return newInstructions.InstructionEnumeration();
        }

        public static bool CallOtherGetTarget(float maxDistance, out GameObject result, out float distance)
        {
            ModVehicle mv = Player.main.GetVehicle() as ModVehicle;
            Exosuit exo = Player.main.GetVehicle() as Exosuit;
            if(mv != null && mv.IsPlayerControlling())
            {
                return Targeting.GetTarget(mv.gameObject, maxDistance, out result, out distance);
            }
            if (exo != null && exo.GetPilotingMode())
            {
                return Targeting.GetTarget(exo.gameObject, maxDistance, out result, out distance);
            }
            return Targeting.GetTarget(Player.main.gameObject, maxDistance, out result, out distance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(PDAScanner.UpdateTarget))]
        public static void PDAScannerUpdateTargetPrefix(float distance, ref bool self)
        {
            if(VehicleFramework.VehicleTypes.Drone.mountedDrone != null)
            {
                self = false;
            }
        }
    }
}
