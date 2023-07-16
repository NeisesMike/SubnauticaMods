using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace PersistentReaper
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Start))]
    class PlayerStartPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.StartCoroutine(GetReaperPrefab(TechType.ReaperLeviathan));
            ReaperManager.LoadPersistentReapers();
        }

        public static IEnumerator GetReaperPrefab(TechType thisTT)
        {
            yield return null;
            TaskResult<GameObject> currentResult = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(thisTT, false, currentResult);
            if (!(currentResult.Get() is null))
            {
                ReaperManager.reaperPrefab = currentResult.Get();
            }
            else
            {
                PersistentReaperPatcher.logger.LogError("Failed to find ReaperLeviathan prefab.");
            }
        }
    }
 
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        private static void leaveScent()
        {
            Int3 thisLoc = ReaperManager.getEcoRegion(Player.main.transform.position);
            // if this isn't a valid location for reapers, skip
            if(!ReaperManager.checkRegionLegality(thisLoc))
            {
                return;
            }
            if (ReaperManager.playerTrailDict.ContainsKey(thisLoc) && ReaperManager.playerTrailDict[thisLoc] != null)
            {
                ReaperManager.playerTrailDict[thisLoc].refreshScent();
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
            if (PersistentReaperPatcher.config.areReapersActive)
            {
                ReaperManager.updateReapers();
                leaveScent();
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnDestroy))]
    public class PlayerOnDestroyPatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            // remove all reaper when player quit to main menu
            // ensure correct behavior between multiple save/load sessions without quiting the game
            ReaperManager.removeAllReapers();
        }
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.OnKill))]
    public class PlayerOnKillPatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (PersistentReaperPatcher.config.shouldRandomOnPlayerDead)
            {
                // remove all reapers when player die, make it initilize random position again when player start
                ReaperManager.removeAllReapers();
            }
        }
    }
}
