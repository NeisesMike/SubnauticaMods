using System.Collections.Generic;
using System.IO;

namespace BZCommon.ConfigurationParser
{
    public class ConfigData : List<ConfigData>
    {
        public string Section { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        public ConfigData(string section, string key, string value)
        {
            Section = section;
            Key = key;
            Value = value;
        }       
    }

    public class SaveData : List<SaveData>
    {
        public string Section { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        public SaveData(string section, string key, string value)
        {
            Section = section;
            Key = key;
            Value = value;
        }
    }

    public class ParserHelper
    {
        public static void CreateDefaultConfigFile(string path, string programName, string version, List<ConfigData> configData)
        {            
            File.WriteAllText(path,$"[{programName}]\r\nVersion: {version}\r\n");

            Parser parser = new Parser(path);

            if (configData == null)
                return;

            foreach (ConfigData data in configData)
            {
                if (!parser.IsExists(data.Section))
                {
                    parser.AddNewSection(data.Section);
                }               
                
                parser.SetKeyValueInSection(data.Section, data.Key, data.Value);
            }            
        }

        public static void CreateSaveGameFile(string path, string programName, string version, List<SaveData> saveDatas)
        {
            File.WriteAllText(path, $"[{programName}]\r\nVersion: {version}\r\n");

            Parser parser = new Parser(path);

            if (saveDatas == null)
                return;

            foreach (SaveData data in saveDatas)
            {
                if (!parser.IsExists(data.Section))
                {
                    parser.AddNewSection(data.Section);
                }

                parser.SetKeyValueInSection(data.Section, data.Key, data.Value);
            }
        }

        public static void AddInfoText(string filename, string key, string value)
        {
            Parser parser = new Parser(filename);

            if (!parser.IsExists("Information"))
                parser.AddNewSection("Information");

            parser.SetKeyValueInSection("Information", key, value);
        }

        public static string GetKeyValue(string filename, string section, string key)
        {
            Parser parser = new Parser(filename);

            if (parser.IsExists(section, key))
            {
                return parser.GetKeyValueFromSection(section, key);
            }
            else
                return string.Empty;
        }        

        public static Dictionary<string, string> GetAllKeyValuesFromSection(string filename, string section, string[] keys)
        {
            Parser parser = new Parser(filename);                

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!parser.IsExists(section))
            {
                result.Add(section, "Error");
                return result;
            }            

            foreach (string key in keys)
            {
                if (parser.IsExists(section, key))
                {
                    result.Add(key, parser.GetKeyValueFromSection(section, key));                    
                }
                else
                    result.Add(section, key);
            }

            return result;
        }

        public static Dictionary<string, string> GetAllKVPFromSection(string filename, string section)
        {
            Parser parser = new Parser(filename);

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!parser.IsExists(section))
            {                
                return result;
            }

            Section _section = parser.GetSection(section);
            /*
            if (_section == null)
            {
                return result;
            }
            */
            foreach (KeyValuePair<string, string> kvp in _section)
            {
              result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }


        public static void SetKeyValue(string filename, string section, string key, string value)
        {
            Parser parser = new Parser(filename);

            if (!parser.IsExists(section))
            {
                parser.AddNewSection(section);
            }

            parser.SetKeyValueInSection(section, key, value);
        }

        public static bool SetAllKeyValuesInSection(string filename, string section, Dictionary<string, string> keyValuePairs)
        {
            Parser parser = new Parser(filename);

            if (!parser.IsExists(section))
            {
                parser.AddNewSection(section);
            }

            if (keyValuePairs.Count == 0)
            {
                parser.ClearAndWrite(section);
                return true;
            }

            foreach (KeyValuePair<string, string> kvp in keyValuePairs)
            {
                parser.SetKeyValueInSection(section, kvp.Key, kvp.Value);
            }

            return true;
        }

        public static bool IsSectionKeysExists(string filename, string section, string[] keys)
        {
            Parser parser = new Parser(filename);

            foreach (string key in keys)
            {
                if (parser.IsExists(section, key))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSectionKeyExists(string filename, string section, string key)
        {
            Parser parser = new Parser(filename);

            return parser.IsExists(section, key) ? true : false;
        }

        public static bool IsSectionExists(string filename, string section)
        {
            Parser parser = new Parser(filename);

            return parser.IsExists(section) ? true : false;
        }

        public static void ClearSection(string filename, string section)
        {
            Parser parser = new Parser(filename);

            parser.ClearAndWrite(section);
        }
    }
}
