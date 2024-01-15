using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options;

namespace SeatruckHotkeys
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[SeatruckHotkeys] " + message);
        }
        public static void Output(string msg)
        {
            Hint main = Hint.main;
            if (main == null)
            {
                return;
            }
            uGUI_PopupMessage message = main.message;
            message.ox = 60f;
            message.oy = 0f;
            message.anchor = TextAnchor.MiddleRight;
            message.SetBackgroundColor(new Color(1f, 1f, 1f, 1f));
            string myMessage = msg;
            message.SetText(myMessage, TextAnchor.MiddleRight);
            message.Show(3f, 0f, 0.25f, 0.25f, null);
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

    public class SeatruckHotkeysPatcher
    {
        internal static MyConfig Config { get; private set; }

        public static void Patch()
        {
            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.seatruckhotkeys.mod");
            harmony.PatchAll();
        }
    }
}
