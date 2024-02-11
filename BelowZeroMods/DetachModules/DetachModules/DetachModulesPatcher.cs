using UnityEngine;
using HarmonyLib;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Utility;
using BepInEx;
using BepInEx.Logging;

namespace SeatruckHotkeys
{
    public static class Logger
    {
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo(message);
        }
        public static void Output(string msg, int x = 500, int y = 0)
        {
            BasicText message = new BasicText(x, y);
            message.ShowMessage(msg, 4);
        }
    }

    public static class HotkeyOptions
    {
        public static bool isDirectEntryDisabled = true;
    }
    [Menu("Seatruck Hotkeys")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Enable Quick Detach")]
        public bool isDetachEnabled = true;
        [Keybind("Detach Modules Button")]
        public KeyCode detachModulesKey = KeyCode.V;

        [Toggle("Enable Direct Exit")]
        public bool isDirectExitEnabled = true;
        [Keybind("Direct Exit Button")]
        public KeyCode directExitKey = KeyCode.R;

        [Toggle("Enable Enter-to-Piloting"), OnChange(nameof(setDirectEntry)), OnGameObjectCreated(nameof(initDirectEntry))]
        public bool isDirectEntryEnabled = true;
        [Keybind("Enter-to-Piloting Button")]
        public KeyCode directEntryKey = KeyCode.V;
        [Toggle("Enable Entry Hinting")]
        public bool isEntryHintingEnabled = true;

        public void setDirectEntry(ToggleChangedEventArgs e)
        {
            HotkeyOptions.isDirectEntryDisabled = !e.Value;
        }
        public void initDirectEntry(GameObjectCreatedEventArgs e)
        {
            HotkeyOptions.isDirectEntryDisabled = !isDirectEntryEnabled;
        }
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    public class SeatruckHotkeysPatcher : BaseUnityPlugin
    {
        internal static MyConfig SHConfig { get; private set; }

        public void Start()
        {
            SHConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID); ;
            harmony.PatchAll();
        }
    }
}
