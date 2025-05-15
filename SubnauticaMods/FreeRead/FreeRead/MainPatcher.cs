using UnityEngine;
using HarmonyLib;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Handlers;
using BepInEx;

namespace FreeRead
{
    [BepInPlugin(GUID, "FreeRead", "2.1.0")]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal const string GUID = "com.mikjaw.subnautica.freeread.mod";
        internal static Config FreeReadConfig { get; private set; }
        public void Start()
        {
            FreeReadConfig = OptionsPanelHandler.RegisterModOptions<Config>();
            new Harmony(GUID).PatchAll();
        }
    }

    [Menu("FreeRead Options")]
    public class Config : ConfigFile
    {
        [Keybind("Open Journal and Auto Move")]
        public KeyCode FreeReadKey = KeyCode.RightControl;
    }
}
