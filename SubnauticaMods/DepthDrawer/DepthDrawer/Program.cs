using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Drawing.Imaging;

namespace DepthDrawer
{
    static class Program
    {
        public static Dictionary<Tuple<int, int>, int> depthMap = getDepthDictionary();

        static void Main()
        {
            var image = new Bitmap("DepthMap.png");

            for (int x = 0; x < 256; x++)
            {
                for (int z = 0; z < 256; z++)
                {
                    image.SetPixel(x, z, getDepthColor(x, z));
                }
            }
            image.Save("DepthDrawer_output_pretty.png", ImageFormat.Png);

            for (int x = 0; x < 256; x++)
            {
                for (int z = 0; z < 256; z++)
                {
                    image.SetPixel(x, z, getDepthColorRaw(x, z));
                }
            }
            image.Save("DepthDrawer_output_raw.png", ImageFormat.Png);

        }

        private static Color getDepthColor(int x, int z)
        {
            int thisDepth = depthMap[new Tuple<int, int>(x, z)];
            Color thisColor;
            if (thisDepth == -1)
            {
                thisColor = Color.FromArgb(128, 0, 0, 0);
            }
            else if (80 < thisDepth && thisDepth < 125)
            {
                // stretch us to the whole range baby
                int stretchedDepth = (thisDepth - 80) * 256 / (45);
                thisColor = Color.FromArgb(128, 0, 0, stretchedDepth);
            }
            else
            {
                thisColor = Color.FromArgb(128, 0, 0, thisDepth);
            }
            return thisColor;
        }

        private static Color getDepthColorRaw(int x, int z)
        {
            int thisDepth = depthMap[new Tuple<int, int>(x, z)];
            Color thisColor;
            if (thisDepth == -1)
            {
                thisColor = Color.FromArgb(128, 0, 0, 0);
            }
            else
            {
                thisColor = Color.FromArgb(128, 0, 0, thisDepth);
            }
            return thisColor;
        }

        private static Dictionary<Tuple<int, int>, int> getDepthDictionary()
        {
            Dictionary<Tuple<int, int>, int> depthDictionary = new Dictionary<Tuple<int, int>, int>();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] dictStringArr = File.ReadAllLines(Path.Combine(modPath, "DepthDictionary.txt"));

            foreach (string entry in dictStringArr)
            {
                Tuple<int, int, int> thisEntry = getDepthEntry(entry);

                Tuple<int, int> thisLocation = new Tuple<int, int>(thisEntry.Item1, thisEntry.Item3);

                depthDictionary.Add(thisLocation, thisEntry.Item2);
            }
            return depthDictionary;
        }

        private static Tuple<int, int, int> getDepthEntry(string entryString)
        {
            string manipString = entryString;

            // kill the leading brackets
            manipString = manipString.Remove(0, 2);

            // get the first number
            string xString = new String(manipString.TakeWhile(Char.IsDigit).ToArray());
            int xDigits = int.Parse(xString);

            // kill the number
            // kill the comma and space
            manipString = manipString.Remove(0, xString.Length + 2);

            // get the second number
            string zString = new String(manipString.TakeWhile(Char.IsDigit).ToArray());
            int zDigits = int.Parse(zString);

            // kill the number
            // kill the bracket, comma, space
            manipString = manipString.Remove(0, zString.Length + 3);

            // get the third number
            string yString = new String(manipString.TakeWhile(Char.IsDigit).ToArray());
            int yDigits = -1;
            if (yString.Length > 0)
            {
                yDigits = int.Parse(yString);
            }

            // return
            return new Tuple<int, int, int>(xDigits, yDigits, zDigits);
        }

    }
}
