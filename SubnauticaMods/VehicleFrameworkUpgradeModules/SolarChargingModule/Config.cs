using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace SolarChargingModule
{
    [Menu("Solar Charging Options")]
    public class Config : ConfigFile
    {
        [Slider("Repeat Rate", Tooltip = "How many tenths of a second the module will wait before adding power again. If you change this setting, remove and re-add solar modules for it to take effect.", Step = 1, DefaultValue = defaultRepeatRate, Min = 1, Max = 100)]
        public int repeatRate = 10;
        const int defaultRepeatRate = 10;
        
        [Slider("Power", Tooltip = "How much power the module will add every time it repeats,  If you change this setting, remove and re-add solar modules for it to take effect.", Step = 1, DefaultValue = defaultPower, Min = 1, Max = 100)]
        public int power = 10;
        const int defaultPower = 10;

        public float GetRepeatRate()
        {
            return (float)repeatRate / (float)defaultRepeatRate;
        }
        public float GetPower()
        {
            return (float)power / (float)defaultPower;
        }
    }
}
