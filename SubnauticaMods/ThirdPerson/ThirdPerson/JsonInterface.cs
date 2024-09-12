using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace ThirdPerson
{
    class JsonInterface
    {
        public static void Write(Dictionary<string, float> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetFilePath(), json);
        }

        public static Dictionary<string, float> Read()
        {
            if (File.Exists(GetFilePath()))
            {
                string json = File.ReadAllText(GetFilePath());
                return JsonConvert.DeserializeObject<Dictionary<string, float>>(json);
            }
            else
            {
                throw new FileNotFoundException($"The file at {GetFilePath()} was not found.");
            }
        }

        public static string GetFilePath()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            return Path.Combine(directoryPath, "config.json");
        }
    }
}
