using BepInEx;

namespace JukeboxLib
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public void Start()
        {
            JukeboxLib.Logger.myLogger = Logger;
            new HarmonyLib.Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
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
