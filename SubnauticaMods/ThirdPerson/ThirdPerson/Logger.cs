using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;

namespace ThirdPerson
{
    public static class Logger
    {
        internal static ManualLogSource MyLog { get; set; }
        public static void Log(string message)
        {
            MyLog.LogInfo(message);
        }
        public static void Warn(string message)
        {
            MyLog.LogWarning(message);
        }
        public static void Error(string message)
        {
            MyLog.LogError(message);
        }
    }
}
