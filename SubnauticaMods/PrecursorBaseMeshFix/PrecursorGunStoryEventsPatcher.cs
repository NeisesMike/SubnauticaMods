using HarmonyLib;

namespace PrecursorBaseMeshFix
{
    [HarmonyPatch(typeof(PrecursorGunStoryEvents))]
    public class PrecursorGunStoryEventsPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PrecursorGunStoryEvents.Start))]
        public static void PrecursorGunStoryEventsStartHarmonyPostfix(PrecursorGunStoryEvents __instance)
        {
            __instance.gameObject.EnsureComponent<MeshManager>();
        }
    }
}
