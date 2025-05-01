using HarmonyLib;
using UWE;
using System.Collections.Generic;

namespace JukeboxLib
{
    [HarmonyPatch(typeof(FreezeTime))]
    public static class FreezeTimePatcher
    {
        private static readonly List<Jukebox> jukeboxes = new List<Jukebox>();
        public static void Register(Jukebox box)
        {
            jukeboxes.RemoveAll(item => item == null);
            jukeboxes.Add(box);
        }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FreezeTime.Set))]
        public static void FreezeTimeSetPostfix(FreezeTime.Id id, float value)
        {
            if(id == FreezeTime.Id.IngameMenu)
            {
                jukeboxes?.RemoveAll(item => item == null);
                if (value > 0.5f)
                {
                    jukeboxes?.ForEach(x => x?.MenuPause(true));
                }
                else
                {
                    jukeboxes?.ForEach(x => x?.MenuPause(false));
                }
            }
        }
    }
}
