using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace JukeboxLib
{
    [HarmonyPatch(typeof(Player))]
    class PlayerPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void PlayerStartHarmonyPostfix()
        {
            JukeboxLibrary.knownDisks.Clear();
            UWE.CoroutineHost.StartCoroutine(CheckDiskStoryGoals());
        }
        private static IEnumerator CheckDiskStoryGoals()
        {
            yield return new WaitUntil(() => Story.StoryGoalManager.main != null);
            JukeboxDisk.displayNames.Keys.ForEach(CheckDiskStoryGoal);
        }
        private static void CheckDiskStoryGoal(TechType tt)
        {
            if (Story.StoryGoalManager.main.IsGoalComplete(JukeboxDisk.BuildStoryGoalString(tt)))
            {
                JukeboxLibrary.knownDisks.Add(JukeboxDisk.displayNames[tt]);
            }
        }
    }
}
