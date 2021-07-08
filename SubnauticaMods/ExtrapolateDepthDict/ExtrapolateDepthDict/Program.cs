using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ExtrapolateDepthDictionary
{
    static class Program
    {
        static Dictionary<Tuple<int, int>, int> depthDictionary;

        static void Main()
        {
            // read in the dictionary
            depthDictionary = getDepthDictionary();

            // cut out the margins
            cutMargins();

            // do stuff
            for(int i=0; i<100; i++)
            {
                Console.WriteLine("Extrapolation: " + i.ToString());
                extrapolateOnce();
            }

            // print out new dictionary
            printDepthDictionary();

            return;
        }

        static void cutMargins()
        {
            // starting from the left, "erase" all columns that are entirely "zeroes"
            for (int i = 0; i < 256; i++)
            {
                bool isAllZeds = true;
                for (int j = 0; j < 256; j++)
                {
                    int thisDepth = depthDictionary[new Tuple<int, int>(i, j)];
                    if ( !(thisDepth == 121 || thisDepth == -1) )
                    {
                        isAllZeds = false;
                        break;
                    }
                }
                if(isAllZeds)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        depthDictionary[new Tuple<int, int>(i, j)] = -1;
                    }
                }
                else
                {
                    break;
                }
            }

            // starting from the right, "erase" all columns that are entirely "zeroes"
            for (int i = 255; 0 <= i; i--)
            {
                bool isAllZeds = true;
                for (int j = 0; j < 256; j++)
                {
                    int thisDepth = depthDictionary[new Tuple<int, int>(i, j)];
                    if (!(thisDepth == 121 || thisDepth == -1))
                    {
                        isAllZeds = false;
                        break;
                    }
                }
                if (isAllZeds)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        depthDictionary[new Tuple<int, int>(i, j)] = -1;
                    }
                }
                else
                {
                    break;
                }
            }

            // starting from the bottom, "erase" all rows that are entirely "zeroes"
            for (int i = 0; i < 256; i++)
            {
                bool isAllZeds = true;
                for (int j = 0; j < 256; j++)
                {
                    int thisDepth = depthDictionary[new Tuple<int, int>(j, i)];
                    if (!(thisDepth == 121 || thisDepth == -1))
                    {
                        isAllZeds = false;
                        break;
                    }
                }
                if (isAllZeds)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        depthDictionary[new Tuple<int, int>(j, i)] = -1;
                    }
                }
                else
                {
                    break;
                }
            }

            // starting from the top, "erase" all rows that are entirely "zeroes"
            for (int i = 255; 0 <= i; i--)
            {
                bool isAllZeds = true;
                for (int j = 0; j < 256; j++)
                {
                    int thisDepth = depthDictionary[new Tuple<int, int>(j, i)];
                    if (!(thisDepth == 121 || thisDepth == -1))
                    {
                        isAllZeds = false;
                        break;
                    }
                }
                if (isAllZeds)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        depthDictionary[new Tuple<int, int>(j, i)] = -1;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        static void extrapolateOnce()
        {
            for(int x=0; x<256; x++)
            {
                for (int z = 0; z < 256; z++)
                {
                    Tuple<int, int> thisLocation = new Tuple<int, int>(x, z);
                    if(depthDictionary[thisLocation] == 121)
                    {
                        depthDictionary[thisLocation] = getMinOfNeighbors(thisLocation);
                    }
                }
            }
        }

        static int getMinOfNeighbors(Tuple<int, int> location)
        {
            List<Tuple<int, int>> locationList = new List<Tuple<int, int>>();

            locationList.Add(new Tuple<int, int>(location.Item1 - 1, location.Item2 - 1));
            locationList.Add(new Tuple<int, int>(location.Item1 - 1, location.Item2));
            locationList.Add(new Tuple<int, int>(location.Item1 - 1, location.Item2 + 1));
            locationList.Add(new Tuple<int, int>(location.Item1, location.Item2 - 1));
            locationList.Add(new Tuple<int, int>(location.Item1, location.Item2 + 1));
            locationList.Add(new Tuple<int, int>(location.Item1 + 1, location.Item2 - 1));
            locationList.Add(new Tuple<int, int>(location.Item1 + 1, location.Item2));
            locationList.Add(new Tuple<int, int>(location.Item1 + 1, location.Item2 + 1));

            bool IsMatching(Tuple<int,int> input)
            {
                // if the location is out of bounds,
                // if the y value is the dummy value,
                // return false

                // if we're on the left side of the map,
                // do NOT extrapolate to the left
                return !(input.Item1 < 0 || input.Item2 < 0 || 256 <= input.Item1 || 256 <= input.Item2
                         || depthDictionary[input] == 121
                         || location.Item1 < 128  && input.Item1 <= location.Item1
                         || location.Item1 >= 128 && input.Item1 >= location.Item1
                         || location.Item2 < 128  && input.Item2 <= location.Item2
                         || location.Item2 >= 128 && input.Item2 >= location.Item2
                         );
            }
            locationList = locationList.Where(x => IsMatching(x)).ToList();

            int sumOfDepths = 0;
            foreach (Tuple<int, int> thisLoc in locationList)
            {
                sumOfDepths += depthDictionary[thisLoc];
            }

            if (sumOfDepths == 0)
            {
                return 121;
            }
            else
            {
                return (sumOfDepths / locationList.Count);
            }
        }

        static Dictionary<Tuple<int, int>, int> getDepthDictionary()
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

        // [(83, 222), 101]
        static Tuple<int, int, int> getDepthEntry(string entryString)
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
            int yDigits = int.Parse(yString);

            // return
            return new Tuple<int, int, int>(xDigits, yDigits, zDigits);
        }

        static void printDepthDictionary()
        {
            string[] dictStringArray = new string[1];
            dictStringArray[0] = string.Join(Environment.NewLine, depthDictionary);

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllLines(Path.Combine(modPath, "DepthDictionary_Extrapolated.txt"), dictStringArray);
        }
    }
}
