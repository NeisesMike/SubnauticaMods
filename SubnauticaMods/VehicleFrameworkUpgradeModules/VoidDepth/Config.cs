using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace VoidDepth
{
    [Menu("Void Depth Module Options")]
    public class Config : ConfigFile
    {
        [Slider("1000s of meters", Tooltip = "How many thousands of meters Crush Depth the Void Depth Module will add.", Step = 1, Min = 0, Max = 25)]
        public int thousands = 5;
        [Slider("100s of meters", Tooltip = "How many hundreds of meters Crush Depth the Void Depth Module will add.", Step = 1, Min = 0, Max = 9)]
        public int hundreds = 0;
        [Toggle("Can be crafted in vanilla fabricator", Tooltip = "Allow the module to be crafted in the vehicle upgrades console and the cyclops fabricator for the cyclops upgrade. Restart required.")]
        public bool vanillaFabricator = false;
    }
    
    public enum EnglishString
    {
        ModuleDisplayName,
        ModuleDescription,
    }
}
