using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.mikjaw.subnautica.updateprofiler.mod", "Update/Coroutine Profiler", "1.0.0")]
public class UpdateProfilerPlugin : BaseUnityPlugin
{
    private Harmony _harmony;
    private static ManualLogSource _log;
    private const int DumpEverySeconds = 10;
    private const int TopN = 25;

    // Limit which assemblies to scan (keeps patch time reasonable)
    // Add your mod assembly name and "Assembly-CSharp" for game scripts.
    //private static readonly string[] TargetAssemblies = { "Assembly-CSharp", "VehicleFramework" };
    private static readonly string[] TargetAssemblies = { "VehicleFramework" };

    private void Awake()
    {
        _log = Logger;
        _harmony = new Harmony("com.mikjaw.subnautica.updateprofiler.mod");
        int methodsPatched = PatchAllUpdates();
        PatchStartCoroutine(); // wraps coroutines
        StartCoroutine(DumpLoop());
        _log.LogInfo($"Profiler ready. Patched {methodsPatched} Update-like methods + StartCoroutine wrapper. " +
                     $"Press F8 to dump, F9 to RESET stats.");
    }

    private void Update()
    {
        // Manual dump hotkey
        if (Input.GetKeyDown(KeyCode.F8))
            StatsStore.DumpTop(_log, TopN, DumpEverySeconds);

        // NEW: reset/clear all accumulated stats
        if (Input.GetKeyDown(KeyCode.F9))
        {
            StatsStore.Reset();
            _log.LogInfo("[Profiler] Stats RESET. New measurements start now.");
        }
    }

    private IEnumerator DumpLoop()
    {
        var wait = new WaitForSeconds(DumpEverySeconds);
        while (true)
        {
            yield return wait;
            StatsStore.DumpTop(_log, TopN, DumpEverySeconds);
        }
    }

    private int PatchAllUpdates()
    {
        var count = 0;
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            string name = asm.GetName().Name ?? "";
            if (!TargetAssemblies.Any(t => string.Equals(t, name, StringComparison.OrdinalIgnoreCase)))
                continue;

            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException rtle) { types = rtle.Types.Where(t => t != null).ToArray(); }

            foreach (var t in types)
            {
                if (t == null || !typeof(MonoBehaviour).IsAssignableFrom(t)) continue;

                foreach (var methName in new[] { "Update", "LateUpdate", "FixedUpdate", "OnGUI" })
                {
                    var m = t.GetMethod(methName, flags, null, Type.EmptyTypes, null);
                    if (m == null || m.IsAbstract) continue;

                    var pre = new HarmonyMethod(typeof(EntryPatch).GetMethod(nameof(EntryPatch.Prefix)));
                    var post = new HarmonyMethod(typeof(EntryPatch).GetMethod(nameof(EntryPatch.Postfix)));
                    _harmony.Patch(m, pre, post);
                    count++;
                }
            }
        }
        return count;
    }

    private void PatchStartCoroutine()
    {
        var target = AccessTools.Method(typeof(MonoBehaviour), nameof(MonoBehaviour.StartCoroutine), new[] { typeof(IEnumerator) });
        var pre = new HarmonyMethod(typeof(CoroutinePatch).GetMethod(nameof(CoroutinePatch.Prefix)));
        _harmony.Patch(target, pre, null);
    }

    // -------- timing store --------
    static class StatsStore
    {
        public class Stats
        {
            public long TotalTicks;
            public long MaxTicks;
            public long Calls;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(long dt)
            {
                Interlocked.Add(ref TotalTicks, dt);
                Interlocked.Increment(ref Calls);
                long cur;
                while (dt > (cur = MaxTicks)) Interlocked.CompareExchange(ref MaxTicks, dt, cur);
            }
        }

        // Make map swappable so we can hard-reset cheaply.
        private static ConcurrentDictionary<string, Stats> _map = new();
        [ThreadStatic] private static Stack<(string key, long start)> _stack;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Begin(string key)
        {
            _stack ??= new Stack<(string, long)>(8);
            _stack.Push((key, Stopwatch.GetTimestamp()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void End()
        {
            if (_stack == null || _stack.Count == 0) return;
            var (key, start) = _stack.Pop();
            long dt = Stopwatch.GetTimestamp() - start;
            var s = _map.GetOrAdd(key, _ => new Stats());
            s.Add(dt);
        }

        public static void AddOnce(string key, long dt)
        {
            var s = _map.GetOrAdd(key, _ => new Stats());
            s.Add(dt);
        }

        public static void Reset()
        {
            // Replace the dictionary reference; old one becomes eligible for GC.
            // This is thread-safe for readers/writers that always fetch _map via the field.
            _map = new ConcurrentDictionary<string, Stats>();
            // Optional: clear the per-thread stack for the calling thread
            _stack?.Clear();
        }

        public static void DumpTop(ManualLogSource log, int topN, int windowSec)
        {
            var snap = _map.ToArray();
            if (snap.Length == 0)
            {
                log.LogInfo($"--- PERF Top {topN}: (no samples yet) ---");
                return;
            }

            double freq = Stopwatch.Frequency;

            var top = snap
                .Select(kv => new
                {
                    Key = kv.Key,
                    Calls = kv.Value.Calls,
                    TotalMs = kv.Value.TotalTicks * 1000.0 / freq,
                    AvgUs = kv.Value.Calls > 0 ? (kv.Value.TotalTicks / (double)kv.Value.Calls) * 1_000_000.0 / freq : 0.0,
                    MaxUs = kv.Value.MaxTicks * 1_000_000.0 / freq
                })
                .OrderByDescending(x => x.TotalMs)
                .Take(topN)
                .ToList();

            log.LogInfo($"--- PERF Top {topN} (since last reset; periodic ~{windowSec}s dumps) ---");
            foreach (var e in top)
                log.LogInfo($"{e.TotalMs,8:F1} ms | {e.Calls,8} calls | {e.AvgUs,7:F1} µs avg | {e.MaxUs,7:F1} µs max | {e.Key}");
        }
    }

    // -------- prefix/postfix for Update-like entry points --------
    public static class EntryPatch
    {
        public static void Prefix(object __instance, MethodBase __originalMethod)
        {
            var key = $"{__instance.GetType().Name}.{__originalMethod.Name}";
            StatsStore.Begin(key);
        }

        public static void Postfix()
        {
            StatsStore.End();
        }
    }

    // -------- coroutine wrapping --------
    public static class CoroutinePatch
    {
        public static void Prefix(MonoBehaviour __instance, ref IEnumerator routine)
        {
            if (routine == null) return;

            string coroName = routine.GetType().Name; // e.g., <MyCoro>d__23
            var moveNext = routine.GetType().GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string srcMethod = moveNext?.DeclaringType?.DeclaringType == __instance.GetType()
                ? InferSourceMethodName(routine.GetType()) ?? coroName
                : coroName;

            string key = $"{__instance.GetType().Name}.Coroutine:{srcMethod}";
            routine = new TimedEnumerator(key, routine);
        }

        private static string InferSourceMethodName(Type t)
        {
            // Examples: <ScanLoop>d__17  -> ScanLoop
            string n = t.Name;
            int s = n.IndexOf('<');
            int e = n.IndexOf('>');
            if (s >= 0 && e > s) return n.Substring(s + 1, e - s - 1);
            return null;
        }

        private sealed class TimedEnumerator : IEnumerator
        {
            private readonly string _key;
            private readonly IEnumerator _inner;

            public TimedEnumerator(string key, IEnumerator inner)
            {
                _key = key;
                _inner = inner;
            }

            public object Current => _inner.Current;

            public bool MoveNext()
            {
                long start = Stopwatch.GetTimestamp();
                bool result = _inner.MoveNext();
                long dt = Stopwatch.GetTimestamp() - start;
                StatsStore.AddOnce(_key, dt);
                return result;
            }

            public void Reset() => _inner.Reset();
        }
    }
}
