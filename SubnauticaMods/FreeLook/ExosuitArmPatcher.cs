using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;

namespace FreeLook
{
    [HarmonyPatch(typeof(ExosuitDrillArm))]
    public class ExosuitDrillArmPatcher
    {
        /* This transpiler makes one part of UpdateEnergyRecharge more generic
         * Optionally cast a ray from exosuit POV instead of player-cursor POV
         */
        [HarmonyPatch(nameof(ExosuitDrillArm.OnHit))]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newCodes = new List<CodeInstruction>(codes.Count);
            CodeInstruction myNOP = new CodeInstruction(OpCodes.Nop);
            for (int i = 0; i < codes.Count; i++)
            {
                newCodes.Add(myNOP);
            }
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().ToLower().Contains("tracefpstargetposition"))
                {
                    newCodes[i] = CodeInstruction.Call(typeof(ExosuitDrillArmPatcher), nameof(ExosuitDrillArmPatcher.GenericRayCastMethod));
                }
                else
                {
                    newCodes[i] = codes[i];
                }
            }
            return newCodes.AsEnumerable();
        }
        public static bool GenericRayCastMethod(GameObject ignoreObject, float maxDistance, ref GameObject closestObject, ref Vector3 position, bool includeUsableTriggers)
        {
            if (ignoreObject.GetComponent<Exosuit>() == null || !Player.main.GetComponent<FreeLookManager>().isFreeLooking)
            {
                // normal behavior
                return UWE.Utils.TraceFPSTargetPosition(ignoreObject, maxDistance, ref closestObject, ref position, includeUsableTriggers);
            }
            else
            {
                Transform signif = ignoreObject.transform.Find("exosuit_01/root/geoChildren/lArm_clav"); //why choose the left arm instead of the right arm?
                RaycastHit[] allHits = Physics.RaycastAll(signif.position, signif.forward, maxDistance);
                var filteredHits = allHits
                    .Where(hit => hit.transform.GetComponent<Creature>() == null) // ignore creatures
                    .Where(hit => hit.transform.GetComponent<Player>() == null) // ignore player
                    ;
                if (0 < filteredHits.Count())
                {
                    GameObject ret = null;
                    float closest = 100;
                    foreach (var hit in filteredHits)
                    {
                        float test = Vector3.Distance(hit.collider.transform.position, Player.main.transform.position);
                        if (test < closest)
                        {
                            closest = test;
                            ret = hit.collider.gameObject;
                        }
                    }
                    closestObject = ret;
                    return true;
                }
                return false;
            }
        }
    }
}
