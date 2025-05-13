using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

namespace JukeboxLib
{
    public static class JukeboxLibrary
    {
        private static int pdaNoteID = 9001;
        private static readonly Dictionary<string, AudioClip> library = new Dictionary<string, AudioClip>();
        internal static readonly List<string> knownDisks = new List<string>();
        internal static Dictionary<string, AudioClip> GetIncludedMusic()
        {
            return library.Where(x => knownDisks.Contains(x.Key))
             .ToDictionary(x => x.Key, x => x.Value);
        }

        #region public_api
        public static AudioClip GetClip(string name)
        {
            if (library.ContainsKey(name))
            {
                return library[name];
            }
            else
            {
                ErrorMessage.AddWarning($"JukeboxLib: There is no audio clip for TechType: {name}");
                return null;
            }
        }
        public static void TryAddClip(string name, AudioClip clip)
        {
            if (library.ContainsKey(name))
            {
                ErrorMessage.AddWarning($"JukeboxLib: There is already an audio clip for TechType: {name}");
            }
            else
            {
                library.Add(name, clip);
            }
        }
        public static void UnlockSong(TechType diskTT)
        {
            string displayName = JukeboxDisk.displayNames[diskTT];
            if (knownDisks.Contains(displayName))
            {
                return; // already unlocked
            }
            knownDisks.Add(displayName);
            if (Subtitles.main != null && Subtitles.main.queue != null)
            {
                Subtitles.main.queue.Add(pdaNoteID++, new StringBuilder($"You unlocked a new jukebox song: \"{displayName}\""), 1f, 3f);
            }
        }
        #endregion
    }
}
