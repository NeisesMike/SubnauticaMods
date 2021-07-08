using System.Collections.Generic;
using System.Text;
using System;

namespace BZCommon.ConfigurationParser
{
    public class Section : Dictionary<string, string>
    {
        public string Name { get; private set; }        

        public Section(string name)
        {
            Name = name;
        }

        public string GetKeyValue(string key)
        {
            try
            {
                return this[key];
            }
            catch
            {
                Console.WriteLine($"Parser Error! [{key}] is missing!");
                return key;
            }
        }        

        public void SetKeyValue(string key, string value)
        {
            this[key] = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat($"[{Name}]\r\n");

            if (Count < 1)
                return sb.ToString();

            foreach (var kvp in this)
                sb.AppendFormat($"{kvp.Key}: {kvp.Value}\r\n");

            return sb.ToString();
        }
    }
}

