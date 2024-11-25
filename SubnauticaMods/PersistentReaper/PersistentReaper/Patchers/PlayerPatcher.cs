using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Start))]
    class PlayerStartPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            UWE.CoroutineHost.StartCoroutine(GetReaperPrefab(TechType.ReaperLeviathan));
            return;
        }

        public static IEnumerator GetReaperPrefab(TechType thisTT)
        {
            yield return null;
            TaskResult<GameObject> currentResult = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(thisTT, false, currentResult);
            if (!(currentResult.Get() is null))
            {
                ReaperManager.ReaperPrefab = currentResult.Get();
            }
            else
            {
                PersistentReaperPatcher.PRLogger.LogError("Failed to find ReaperLeviathan prefab.");
            }
        }
    }
 
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Update))]
    public class PlayerUpdatePatcher
    {
        private static void LeaveScent()
        {
            Int3 thisLoc = ReaperManager.GetEcoRegion(Player.main.transform.position);
            // if this isn't a valid location for reapers, skip
            if(!ReaperManager.CheckRegionLegality(thisLoc))
            {
                return;
            }
            if (ReaperManager.playerTrailDict.ContainsKey(thisLoc) && ReaperManager.playerTrailDict[thisLoc] != null)
            {
                ReaperManager.playerTrailDict[thisLoc].RefreshScent();
            }
            else
            {
                Scent thisScent = new Scent();
                ReaperManager.playerTrailDict[thisLoc] = thisScent;
            }
        }
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (PersistentReaperPatcher.PRConfig.areReapersActive)
            {
                ReaperManager.UpdateReapers();
                LeaveScent();
            }
        }
    }
}
