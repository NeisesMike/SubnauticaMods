using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UWE;

namespace GroundedItems
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Start))]
    class PlayerPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            AddItemGrounderToPrefabOfTechType(TechType.StalkerTooth);
            AddItemGrounderToPrefabOfTechType(TechType.ScrapMetal);
        }

        public static void AddItemGrounderToPrefabOfTechType(TechType thisTT)
        {
            // add the ItemGrounder component to the StalkerTooth prefab
            string prefabFileName;
            var ttClassID = CraftData.GetClassIdForTechType(thisTT);
            var thisPrefab = CraftData.GetPrefabForTechType(thisTT);
            PrefabDatabase.TryGetPrefabFilename(ttClassID, out prefabFileName);
            thisPrefab.EnsureComponent<ItemGrounder>();

            // set the cell-level to far,
            // to ensure the tooth can fall far "out-of-bounds,"
            // before being disabled by getting too far from player
            thisPrefab.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;

            // One of these might be unnecessary :shrug:
            PrefabDatabase.AddToCache(prefabFileName, thisPrefab);
            PrefabDatabase.prefabFiles[ttClassID] = prefabFileName;
            ScenePrefabDatabase.scenePrefabs[ttClassID] = thisPrefab;
        }
    }
}
