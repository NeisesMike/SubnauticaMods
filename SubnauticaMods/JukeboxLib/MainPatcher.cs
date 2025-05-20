using BepInEx;
using UnityEngine;
using Nautilus.Utility;
using System.Reflection;

namespace JukeboxLib
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static AssetBundle AssetBundle = null;
        public void Start()
        {
            AssetBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "jukeboxdisk_assets");
            if (AssetBundle == null)
            {
                ErrorMessage.AddError("JukeboxLib: Failed to fetch asset bundle. See log for details.");
                throw new System.Exception("JukeboxLib: Failed to fetch asset bundle. Double check existence of jukeboxDisk_assets.asset_bundle!");
            }
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
