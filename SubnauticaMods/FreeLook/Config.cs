using UnityEngine;
using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace FreeLook
{
    [Menu("FreeLook Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Enable Hinting")]
        public bool isHintingEnabled = true;

        [Keybind("FreeLook Key")]
        public KeyCode FreeLookKey = KeyCode.LeftAlt;

        [Toggle("Toggle FreeLook", Tooltip = "Enable this to have FreeLook toggle instead of requiring you to hold the button to FreeLook.")]
        public bool isToggle = false;

        [Slider("Trigger Deadzone %", Tooltip = "Add deadzone to the freelook input on analog-based triggers. Higher number means more deadzone.", Min = 0, Max = 100, Step = 1)]
        public int deadzone = 20;
    }
}
