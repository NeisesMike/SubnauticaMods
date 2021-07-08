using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace DepthDrawer
{
    static class Program
    {
        public static Dictionary<Tuple<int, int>, int> depthDictionary = new Dictionary<Tuple<int, int>, int>();

        static void Main()
        {
            var image = new Bitmap("DepthMap_input.png");
            for (int x = 0; x < 256; x++)
            {
                for (int z = 0; z < 256; z++)
                {
                    int thisDepth = image.GetPixel(x, z).B;
                    if (thisDepth == 0)
                    {
                        thisDepth = -1;
                    }
                    depthDictionary.Add(new Tuple<int, int>(x, z), thisDepth);
                }
            }
            printDepthDictionary();
        }

        public static void printDepthDictionary()
        {
            string[] dictStringArray = new string[1];
            dictStringArray[0] = string.Join(Environment.NewLine, depthDictionary);

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllLines(Path.Combine(modPath, "DepthDictionary_output.txt"), dictStringArray);
        }

    }
}
