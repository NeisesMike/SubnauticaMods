using BepInEx.Configuration;
using System;

namespace SimpleMainMenu
{
    internal class SimpleMainMenuConfig
    {
        internal SimpleMainMenuConfig()
        {
            EnableRightSide = MainPatcher.Instance.Config.Bind<bool>("Options", "Re-enable menus", false, "Toggle this to enable/disable the menus this mod controls.");
            EnableRightSide.SettingChanged += EnableRightSideAction;
        }
        void EnableRightSideAction(object sender, EventArgs e)
        {
            if(MainMenuRightSide.main != null)
            {
                MainMenuRightSide.main.OpenGroup("Home");
            }
        }
        internal ConfigEntry<bool> EnableRightSide { get; set; }
    }
}
