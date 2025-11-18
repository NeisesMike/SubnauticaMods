using HarmonyLib;
using Nautilus.Handlers;
using BepInEx;

namespace FreeRead
{
    [BepInPlugin(GUID, "FreeRead", "4.0.1")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal const string GUID = "com.mikjaw.subnautica.freeread.mod";
        internal static MainPatcher Instance { get; private set; } = null;
        private void Awake()
        {
            SetupInstance();
        }
        public void Start()
        {
            new Harmony(GUID).PatchAll();
        }

        internal GameInput.Button ToggleFreeReadKey = EnumHandler.AddEntry<GameInput.Button>("Toggle FreeRead PDA")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("<Keyboard>/tab")
            .AvoidConflicts(GameInput.Device.Keyboard)
            .WithCategory("Free Read");

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
