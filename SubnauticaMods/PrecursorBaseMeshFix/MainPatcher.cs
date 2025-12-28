
namespace PrecursorBaseMeshFix
{
    [BepInEx.BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MainPatcher : BepInEx.BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.mikjaw.subnautica.precursormeshfix.mod";
        public const string PLUGIN_NAME = "Precursor Mesh Fix";
        public const string PLUGIN_VERSION = "1.0.0";
        public void Start()
        {
            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll(typeof(PrecursorGunStoryEventsPatcher));
        }
    }
}
