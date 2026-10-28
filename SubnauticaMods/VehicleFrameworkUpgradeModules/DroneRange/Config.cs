using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace DroneRange
{
    [Menu("Drone Range Options")]
    public class Config : ConfigFile
    {
        [Slider("Range Bonus", Tooltip = "Additional drone connection range per module. Default is 200.", Step = 50, Min = 0, Max = 1000)]
        public int rangeBonus = 200;
    }
}
