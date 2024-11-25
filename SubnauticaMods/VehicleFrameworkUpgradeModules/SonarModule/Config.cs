using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace SonarModule
{
    [Menu("Sonar Module Options")]
    public class Config : ConfigFile
    {
        [Slider("Repeat Rate", Tooltip = "How many seconds the module will wait before pinging again. Default is 5.", Step = 1, Min = 1, Max = 10)]
        public int repeatRate = 5;
        
        [Slider("Ping Duration", Tooltip = "How many seconds the ping will take to complete. Default is 5.", Step = 1, Min = 1, Max = 10)]
        public int duration = 5;
    }
}
