using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;

namespace ThirdPerson
{
    class JsonInterface
    {
        public static void Write(Dictionary<string, float> distances, Dictionary<string, float> pitches)
        {
            List<string> allNames = new List<string>();
            distances.ForEach(x => allNames.Add(x.Key));
            pitches.ForEach(x => allNames.Add(x.Key));
            List<Tuple<string, float, float>> result = new List<Tuple<string, float, float>>();
            foreach (string name in allNames.Distinct())
            {
                float distance = distances.GetOrDefault(name, PerVehicleConfig.defaultZoom);
                float pitch = pitches.GetOrDefault(name, PerVehicleConfig.defaultPitch);
                result.Add(new Tuple<string, float, float>(name, distance, pitch));
            }
            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(GetFilePath(), json);
        }
        public static List<Tuple<string, float, float>> ReadAll()
        {
            if (File.Exists(GetFilePath()))
            {
                string json = File.ReadAllText(GetFilePath());
                return JsonConvert.DeserializeObject<List<Tuple<string, float, float>>>(json);
            }
            else
            {
                throw new FileNotFoundException($"The file at {GetFilePath()} was not found.");
            }
        }
        public static Dictionary<string, float> ReadDistances()
        {
            var config = ReadAll();
            Dictionary<string, float> result = new Dictionary<string, float>();
            foreach(var marty in config)
            {
                result.Add(marty.Item1, marty.Item2);
            }
            return result;
        }
        public static Dictionary<string, float> ReadPitches()
        {
            var config = ReadAll();
            Dictionary<string, float> result = new Dictionary<string, float>();
            foreach (var marty in config)
            {
                result.Add(marty.Item1, marty.Item3);
            }
            return result;
        }
        public static string GetFilePath()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            return Path.Combine(directoryPath, "config.json");
        }
    }
}
