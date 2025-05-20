using BepInEx;
using System.Collections;

namespace SimpleJukebox
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(JukeboxLib.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.mikjaw.subnautica.desktopjukebox.mod";
        public const string PLUGIN_NAME = "Desktop Jukebox";
        public const string PLUGIN_VERSION = "1.0.0";
        public IEnumerator Start()
        {
            AssetLoader.LoadRadioAsset();
            DesktopJukebox.RegisterJukebox();
            yield return UWE.CoroutineHost.StartCoroutine(DesktopJukebox.LoadMasterPlaylist());
            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll();
        }
    }
}

