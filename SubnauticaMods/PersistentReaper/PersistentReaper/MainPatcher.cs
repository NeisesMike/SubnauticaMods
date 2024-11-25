using HarmonyLib;
using Nautilus.Handlers;
using BepInEx;
using BepInEx.Logging;

namespace PersistentReaper
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class PersistentReaperPatcher : BaseUnityPlugin
    {
        public static ManualLogSource PRLogger { get; private set; }
        internal static MyConfig PRConfig { get; private set; }
        public void Start()
        {
            PRLogger = base.Logger;
            PRConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}
