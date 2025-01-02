using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace RollControl
{
    [Menu("RollControl Options")]
    public class MyConfig : ConfigFile
    {
        [Keybind("Toggle Roll Key"), Tooltip("Roll is toggled individually for scuba mode and for each vehicle you have.")]
        public KeyCode ToggleRollKey = KeyCode.RightAlt;
        [Keybind("Roll Counter-Clockwise")]
        public KeyCode RollPortKey = KeyCode.Z;
        [Keybind("Roll Clockwise")]
        public KeyCode RollStarboardKey = KeyCode.C;
        [Slider("Submarine Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 30f)]
        public double SubmarineRollSpeed = 30f;
        [Slider("Scuba Roll Speed", Min = 0f, Max = 100f, Step = 1f, DefaultValue = 75f)]
        public double ScubaRollSpeed = 75f;
        [Slider("Scuba Mouse Sensitivity", Min = 0f, Max = 200f, Step = 1f, DefaultValue = 30f, Tooltip = "How fast the camera rotates as you move the mouse")]
        public float ScubaMouseSensitivity = 30f;
        [Slider("Scuba Controller Sensitivity", Min = 0f, Max = 200f, Step = 1f, DefaultValue = 30f, Tooltip = "How fast the camera rotates as you tilt the analog stick")]
        public float ScubaPadSensitivity = 15f;
        [Toggle("Enable Vehicle Roll by Default")]
        public bool IsVehicleRollDefaultEnabled = false;
        [Toggle("Enable Scuba Roll by Default")]
        public bool IsScubaRollDefaultEnabled = false;
        [Toggle("Enable Scuba Hint", Tooltip = "This will display a reminder message on-screen.")]
        public bool IsScubaHinting = false;
    }
}
