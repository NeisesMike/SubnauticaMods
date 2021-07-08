using System;
using System.IO;

namespace BZCommon.ConfigurationParser
{
    public class FileReader
    {
        public string FilePath { get; private set; }
        public string Contents { get; private set; }
        public string[] Lines { get; private set; }

        public FileReader(string filePath)
        {
            if (!File.Exists(filePath))                
                throw new FileNotFoundException(string.Format("File {0} does not exist", filePath));                           

            FilePath = filePath;
            using (var reader = new StreamReader(FilePath))
                Contents = reader.ReadToEnd().Trim();
            Lines = Contents.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }       
    }
}

