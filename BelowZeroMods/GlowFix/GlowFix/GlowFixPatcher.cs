using System.Collections.Generic;
using HarmonyLib;

namespace GlowFix
{
    public static class Logger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log("[GlowFix] " + message);
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log("[GlowFix] " + string.Format(format, args));
        }
    }

    public class GlowFixPatcher
    {
        internal static List<TechType> exteriorModuleTechTypes;
        public static void Patch()
        {
            var harmony = new Harmony("com.garyburke.subnautica.glowfix.mod");
            harmony.PatchAll();
        }
    }
}

