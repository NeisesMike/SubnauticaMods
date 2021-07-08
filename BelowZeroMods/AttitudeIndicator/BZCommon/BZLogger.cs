using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BZCommon
{
    public enum LogMode
    {
        LOG,
        WARNING,
        ERROR,
        DEBUG
    }

    public static class BZLogger
    {
        private static readonly Dictionary<LogMode, string> logTypeCache = new Dictionary<LogMode, string>(4)
        {
            { LogMode.LOG, "LOG" },
            { LogMode.WARNING, "WARNING" },
            { LogMode.ERROR, "ERROR" },
            { LogMode.DEBUG, "DEBUG" },
        };

        private static void WriteLog(LogMode logType, string message)
        {
            Console.WriteLine($"[{Assembly.GetCallingAssembly().GetName().Name}/{logTypeCache[logType]}] {message}");
        }

        public static void Log(string message) => WriteLog(LogMode.LOG, message);

        public static void Log(string format, params object[] args) => WriteLog(LogMode.LOG, string.Format(format, args));

        public static void Warn(string message) => WriteLog(LogMode.WARNING, message);

        public static void Warn(string format, params object[] args) => WriteLog(LogMode.WARNING, string.Format(format, args));

        public static void Error(string message) => WriteLog(LogMode.ERROR, message);

        public static void Error(string format, params object[] args) => WriteLog(LogMode.ERROR, string.Format(format, args));

        [Conditional("DEBUG")]
        public static void Debug(string message) => WriteLog(LogMode.DEBUG, message);

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args) => WriteLog(LogMode.DEBUG, string.Format(format, args));
    }
}
