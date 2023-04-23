using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UWE;
using UnityEngine;

namespace GroundedItems
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Start))]
    class PlayerPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.StartCoroutine(AddItemGrounderToPrefabOfTechType(TechType.StalkerTooth));
            __instance.StartCoroutine(AddItemGrounderToPrefabOfTechType(TechType.ScrapMetal));
        }

        public static IEnumerator bingodingo()
        {
            TaskResult<GameObject> currentResult = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(TechType.ReaperLeviathan, false, currentResult);
            GameObject thisPrefab = currentResult.Get();
            MainPatcher.logger.LogWarning(thisPrefab.name);
        }

        public static IEnumerator AddItemGrounderToPrefabOfTechType(TechType thisTT)
        {
            yield return null;
                        // add the ItemGrounder component to the StalkerTooth prefab
            string prefabFileName;
            var ttClassID = CraftData.GetClassIdForTechType(thisTT);
            TaskResult<GameObject> currentResult = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(thisTT, false, currentResult);
            GameObject thisPrefab = currentResult.Get();
            PrefabDatabase.TryGetPrefabFilename(ttClassID, out prefabFileName);
            thisPrefab.EnsureComponent<ItemGrounder>();

            // set the cell-level to far,
            // to ensure the tooth can fall far "out-of-bounds,"
            // before being disabled by getting too far from player
            thisPrefab.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;

            // One of these might be unnecessary :shrug:
            //PrefabDatabase.AddToCache(prefabFileName, thisPrefab); // this call no longer exists.
            PrefabDatabase.prefabFiles[ttClassID] = prefabFileName;
            ScenePrefabDatabase.scenePrefabs[ttClassID] = thisPrefab;

        }
    }
}
