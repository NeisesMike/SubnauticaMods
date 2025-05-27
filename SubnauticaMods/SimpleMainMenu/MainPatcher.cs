namespace SimpleMainMenu
{
    [BepInEx.BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MainPatcher : BepInEx.BaseUnityPlugin
    {
        const string PLUGIN_GUID = "com.mikjaw.subnautica.simplemainmenu.mod";
        const string PLUGIN_NAME = "Simple Main Menu";
        const string PLUGIN_VERSION = "1.0.2";
        public static MainPatcher Instance { get; private set; }
        internal static SimpleMainMenuConfig SimpleMainMenuConfig { get; private set; }
        public void Awake()
        {
            SetupInstance();
            SimpleMainMenuConfig = new SimpleMainMenuConfig();
        }
        public void Start()
        {
            new HarmonyLib.Harmony(PLUGIN_GUID).PatchAll(typeof(uGUI_MainMenuPatcher));
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
    }
}
