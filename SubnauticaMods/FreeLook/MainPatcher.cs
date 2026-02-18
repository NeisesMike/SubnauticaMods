using System;
using HarmonyLib;
using Nautilus.Handlers;
using BepInEx;

namespace FreeLook
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class FreeLookPatcher : BaseUnityPlugin
    {
        internal static MyConfig config { get; private set; }
        internal static FreeLookPatcher Instance { get; private set; } = null;
        private void Awake()
        {
            SetupInstance();
        }

        public void Start()
        {
            config = OptionsPanelHandler.RegisterModOptions<MyConfig>();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // here we coerce tweaks and fixes into compatibility
            // in other words, we neuter one of its patches.
            var type2 = Type.GetType("Tweaks_Fixes.SeaMoth_patch, Tweaks and Fixes", false, false);
            if (type2 != null)
            {
                var TweaksFixesSeamothUpdatePrefix = AccessTools.Method(type2, "UpdatePrefix");
                harmony.Patch(TweaksFixesSeamothUpdatePrefix, prefix: new HarmonyMethod(typeof(FreeLookPatcher), nameof(TweaksFixesSeamothUpdatePrefixPrefix)));
            }
        }

        public static bool TweaksFixesSeamothUpdatePrefixPrefix(SeaMoth __instance, ref bool __result)
        {
            __result = true;
            return false;
        }

        internal GameInput.Button FreeLookKey = EnumHandler.AddEntry<GameInput.Button>("Engage FreeLook")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/leftalt")
            .AvoidConflicts(GameInput.Device.Keyboard)
            .WithCategory("Free Look");

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
    }
}
