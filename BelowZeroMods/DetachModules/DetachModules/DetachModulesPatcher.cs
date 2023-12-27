using UnityEngine;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Options;
using System.Runtime.CompilerServices;
using System.Reflection;

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
        public MyConfig() : base("seatruck_hotkeys", "") { }
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

    [BepInPlugin(MyGuid, PluginName, VersionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class SeatruckHotkeysPatcher : BaseUnityPlugin
    {
        private const string MyGuid = "com.mikjaw.subnautica.seatruckhotkeys.mod";
        private const string PluginName = "Seatruck Hotkeys";
        private const string VersionString = "1.3.2";
		
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static ManualLogSource Log { get; private set;}
		
        internal static new MyConfig Config { get; private set; }
		
		

        private void Awake()
        {
            Log = base.Logger;
            Config = OptionsPanelHandler.RegisterModOptions<MyConfig>();

            Harmony.CreateAndPatchAll(Assembly, MyGuid);
            Log.LogInfo(PluginName + " " + VersionString + " " + "loaded.");
        }
    }
}
