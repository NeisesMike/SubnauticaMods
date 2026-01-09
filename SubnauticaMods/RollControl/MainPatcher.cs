using System;
using Nautilus.Handlers;
using HarmonyLib;
using Nautilus.Utility;
using BepInEx;
using BepInEx.Logging;

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

    [BepInPlugin("com.mikjaw.subnautica.rollcontrol.mod", "RollControl", "5.4.2")]
    [BepInDependency("com.snmodding.nautilus")]
    public partial class MainPatcher : BaseUnityPlugin
    {
        internal static MainPatcher Instance { get; private set; } = null;
        internal static MyConfig RCConfig { get; private set; }
        internal static bool ThereIsVehicleFramework = false;
        internal void Awake()
        {
            SetupInstance();
        }
        internal void Start()
        {
            RollControl.Logger.MyLog = base.Logger;
            RCConfig = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
            var type = Type.GetType("VehicleFramework.MainPatcher, VehicleFramework", false, false);
            if (type != null)
            {
                ThereIsVehicleFramework = true;
            }
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

        internal GameInput.Button ToggleRollKey = EnumHandler.AddEntry<GameInput.Button>("Toggle Roll")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/m")
            .WithCategory("Roll Control");

        internal GameInput.Button RollPortKey = EnumHandler.AddEntry<GameInput.Button>("Roll Left")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/z")
            .WithCategory("Roll Control");

        internal GameInput.Button RollStarboardKey = EnumHandler.AddEntry<GameInput.Button>("Roll Right")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/c")
            .WithCategory("Roll Control");
    }
}
