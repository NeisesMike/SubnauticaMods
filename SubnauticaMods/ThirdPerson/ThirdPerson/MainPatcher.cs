using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

namespace ThirdPerson
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        public static MainPatcher Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            if (Instance != this)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }
        public void Start()
        {
            ThirdPerson.Logger.MyLog = base.Logger;
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            ThirdPerson.Config.SetupAll();
        }
        public ConfigEntry<KeyboardShortcut> EnableThirdPerson { get; internal set; }
        public ConfigEntry<KeyboardShortcut> EnableConfigurationMode { get; internal set; }
        public ConfigEntry<bool> EnableUpDownChord { get; internal set; }
    }
}
