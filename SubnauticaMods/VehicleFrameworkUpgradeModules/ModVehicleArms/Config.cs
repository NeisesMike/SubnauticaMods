using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace VFDrillArm
{
    [Menu("VF Drill Arm Options")]
    public class Config : ConfigFile
    {
        const string optionText =
            "Disable this to NOT have a Prawn version of this upgrade. " +
            "This Prawn drill arm has the same recipe as the normal drill arm, " +
            "but it's built at the Modification station and can be toggled. " +
            "If you change this option, please restart the game for it to take effect!";
        [Toggle("Prawn Arm", Tooltip = optionText)]
        public bool isPrawnArm = true;
    }
}
