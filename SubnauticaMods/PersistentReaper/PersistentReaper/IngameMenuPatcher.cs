using HarmonyLib;

namespace PersistentReaper
{
    
    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.SaveGame))]
    public class IngameMenuPatcher
    {
        [HarmonyPostfix]
        public static void PreFix()
        {
            ReaperManager.SavePersistentReapers();
            FixCachedReaper();
        }
        
        // There is a problem with spawned Mod Reapers being saved to Cached Reaper.
        // It leads to an inconsistent number of reapers spawned by the mod.
        // For example, let's assume the number of reapers to be added to the game is set to 10.
        // If two of them are close enough to the player, they will be spawned.
        // If the player saves the game, these two will be cached.
        // After reloading the saved game,
        //     these 2 Cached Reapers will spawn by the game.
        //     However, the mod doesn't recognize these 2 Cached Reapers.
        // Therefore, we get a total of 12 reapers in the game:
        //     10 Mod Reapers (2 of which are close enough and, therefore, will also spawn) 
        //     2 Cached Reapers at that location.
        // This number will keep increasing if Mod Reapers are saved to cache,
        //     until the cache is cleared manually or by the game?
        // Also, when loading into a saved game file with Cached Reapers, the mod doesn't have control over those reapers.
        //      Maybe if there's a check in ReaperPatcher to know that it's previously been cached?
        private static void FixCachedReaper()
        {
            // prevent our spawned reapers to be saved in a game cached
            // for now, just remove them before saving, then they should be spawned back automatically by reaper update  
            ReaperManager.despawnAllReapers();
        }
    }
}