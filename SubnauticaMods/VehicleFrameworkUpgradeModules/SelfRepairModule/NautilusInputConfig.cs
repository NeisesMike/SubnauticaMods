
using Nautilus.Handlers;

namespace SelfRepairModule
{
    public partial class MainPatcher
    {
        const string toggleButton = "SelfRepairModuleToggleButton";
        internal GameInput.Button ToggleKey = EnumHandler.AddEntry<GameInput.Button>(toggleButton)
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.R)
            .AvoidConflicts(GameInput.Device.Keyboard)
            .WithCategory("Self-Repair Module");
    }
}
