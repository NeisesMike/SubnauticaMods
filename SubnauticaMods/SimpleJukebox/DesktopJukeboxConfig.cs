using BepInEx.Configuration;
using UnityEngine;

namespace SimpleJukebox
{
    internal class DesktopJukeboxConfig
    {
        internal DesktopJukeboxConfig()
        {
            ReverbPreset = MainPatcher.Instance.Config.Bind<AudioReverbPreset>("Audio Options", "Reverb Preset", AudioReverbPreset.Stoneroom, "Set the reverb preset for the Desktop Jukebox.");
        }
        internal ConfigEntry<AudioReverbPreset> ReverbPreset { get; set; }
    }
}

