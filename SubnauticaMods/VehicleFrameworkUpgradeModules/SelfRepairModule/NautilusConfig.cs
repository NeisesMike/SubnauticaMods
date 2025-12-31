using Nautilus.Options.Attributes;

namespace SelfRepairModule
{
    public enum RepairTypes
    {
        Passive,
        Toggle,
    }

    [Menu("Self-Repair Module Options")]
    class NautilusConfig : Nautilus.Json.ConfigFile
    {
        // Note to self: don't do it this way. It would probably be better to register this upgrade as passive or toggleable at the VF level.
        [Choice("Repair Type", Tooltip = "MODES: Passive: Automatically repairs your vehicles. Toggle: Allows you to toggle repair on and off (see Mod Input menu for keybind).")]
        public RepairTypes repairType = RepairTypes.Passive;

        [Slider("Minimum Energy Requirement", Format = "{0:F1}%", DefaultValue = 20f, Min = 0f, Max = 70f, Step = 2.5f, Tooltip = "Minimum % of energy required to allow self-repairing")]
        public float energyRequirement = 20f;

        [Slider("Stunned Period", DefaultValue = 5f, Min = 0f, Max = 20f, Step = 1f, Tooltip = "After a vehicle has taken damage the repair system will be stunned for this long (does not affect upgrades)")]
        public float stunnedTime = 5f;

        [Toggle("Play Repair Sound")]
        public bool repairSound = true;

        [Toggle("Works below Crush-Depth")]
        public bool crushDepth = false;
    }
}
