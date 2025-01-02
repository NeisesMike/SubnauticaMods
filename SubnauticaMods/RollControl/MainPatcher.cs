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

    [BepInPlugin("com.mikjaw.subnautica.rollcontrol.mod", "RollControl", "5.2")]
    [BepInDependency("com.snmodding.nautilus")]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        public static bool ThereIsVehicleFramework = false;
        public void Start()
        {
            RollControl.Logger.MyLog = base.Logger;
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony("com.mikjaw.subnautica.rollcontrol.mod");
            harmony.PatchAll();
            var type = Type.GetType("VehicleFramework.MainPatcher, VehicleFramework", false, false);
            if (type != null)
            {
                ThereIsVehicleFramework = true;
            }
        }
    }
}
