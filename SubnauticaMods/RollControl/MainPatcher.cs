using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using static VFXParticlesPool;

namespace RollControl
{
    public static class Logger
    {
        public static void Output(string msg)
        {
            BasicText message = new BasicText(500, -75);
            message.ShowMessage(msg, 5);
        }
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo("[RollControl] " + message);
        }
    }

    [BepInPlugin(Plugin_GUID, Plugin_NAME, Plugin_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public partial class MainPatcher : BaseUnityPlugin
    {
        private const string Plugin_GUID = "com.mikjaw.subnautica.rollcontrol.mod";
        private const string Plugin_NAME = "RollControl";
        private const string Plugin_VERSION = "5.4.5";
        internal static MyConfig RCConfig { get; private set; }
        internal static bool ThereIsVehicleFramework = false;
        internal void Start()
        {
            RollControl.Logger.MyLog = base.Logger;
            RCConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(Plugin_GUID);
            harmony.PatchAll();
            var type = Type.GetType("VehicleFramework.MainPatcher, VehicleFramework", false, false);
            if (type != null)
            {
                ThereIsVehicleFramework = true;
            }
        }
#if SN1
        internal static MainPatcher Instance { get; private set; } = null;
        internal void Awake()
        {
            Nautilus.Handlers.LanguageHandler.RegisterLocalizationFolder();
            SetupInstance();
        }
        private void SetupInstance()
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
        const string toggleRollKeyButtonName = "ToggleRoll";
        const string rollPortKeyButtonName = "PortRoll";
        const string rollStarboardKeyButtonName = "StarboardRoll";

        internal GameInput.Button ToggleRollKey = EnumHandler.AddEntry<GameInput.Button>(toggleRollKeyButtonName)
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/m")
            .WithCategory("Roll Control");

        internal GameInput.Button RollPortKey = EnumHandler.AddEntry<GameInput.Button>(rollPortKeyButtonName)
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/z")
            .WithCategory("Roll Control");

        internal GameInput.Button RollStarboardKey = EnumHandler.AddEntry<GameInput.Button>(rollStarboardKeyButtonName)
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/c")
            .WithCategory("Roll Control");
    }
#endif
}
