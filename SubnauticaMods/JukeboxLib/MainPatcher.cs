using BepInEx;
using System.Collections;
using System.Reflection;
using System.IO;

namespace JukeboxLib
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public IEnumerator Start()
        {
            Logger.LogInfo("JukeboxLib Starting!");
            AssetLoader.LoadRadioAsset();
            JukeboxLib.Logger.myLogger = Logger;
            DefaultJukebox.RegisterJukebox();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(modPath, "music");
            yield return UWE.CoroutineHost.StartCoroutine(DefaultJukebox.LoadMasterPlaylist(fullPath));
            new HarmonyLib.Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
            JukeboxLib.Logger.Log("JukeboxLib Finished!");
        }
    }

    internal static class Logger
    {
        internal static BepInEx.Logging.ManualLogSource myLogger;
        internal static void Log(string message)
        {
            myLogger.LogInfo(message);
        }
    }
}
