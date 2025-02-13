using BepInEx.Configuration;

namespace ThirdPerson
{
    public static class Config
    {
		public static void SetupAll()
        {
			SetupThirpyButton();
			SetupConfigButton();
			SetupThirpyButtonEnabled();
        }
		public static void SetupThirpyButton()
		{
			ConfigFile config = MainPatcher.Instance.Config;
			string section = "General";
			string key = "Third Person Keyboard Button";
			string description = "When pressed, will enable and disable third person mode.";
			MainPatcher.Instance.EnableThirdPerson = config.Bind<KeyboardShortcut>(section, key, KeyboardShortcut.Empty, description);
		}
		public static void SetupConfigButton()
		{
			ConfigFile config = MainPatcher.Instance.Config;
			string section = "General";
			string key = "Third Person Configuration Keyboard Button";
			string description = "When pressed, will enable and disable the third person configuration mode.";
			MainPatcher.Instance.EnableConfigurationMode = config.Bind<KeyboardShortcut>(section, key, KeyboardShortcut.Empty, description);
		}
		public static void SetupThirpyButtonEnabled()
		{
			ConfigFile config = MainPatcher.Instance.Config;
			string section = "General";
			string key = "Enable Original Control Scheme";
			string description = "Enable using the move-up and move-down buttons at the same time to control the camera";
			MainPatcher.Instance.EnableUpDownChord = config.Bind<bool>(section, key, true, description);
		}
	}
}
