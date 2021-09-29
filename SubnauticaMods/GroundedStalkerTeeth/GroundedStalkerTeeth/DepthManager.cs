using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace GroundedItems
{
    public static class DepthManager
    {
        public static Dictionary<Tuple<int, int>, int> depth_dictionary;
        public static Dictionary<Tuple<int, int>, int> GetDepthDictionary()
        {
            Tuple<int, int, int> getDepthEntry(string entryString)
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

            Dictionary<Tuple<int, int>, int> depthDictionary = new Dictionary<Tuple<int, int>, int>();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string depthMapString = "DepthDictionary.txt";
            string[] dictStringArr = File.ReadAllLines(Path.Combine(modPath, depthMapString));

            foreach (string entry in dictStringArr)
            {
                Tuple<int, int, int> thisEntry = getDepthEntry(entry);
                Tuple<int, int> thisLocation = new Tuple<int, int>(thisEntry.Item1, thisEntry.Item3);
                depthDictionary.Add(thisLocation, thisEntry.Item2);
            }
            return depthDictionary;
        }
        public static Int3 GetEcoRegion(Vector3 pos)
        {
            Int3 result = Int3.zero;

            Bounds ecoRegionsBounds;
            float num = 256f * 16f * 0.5f;
            float num2 = 128f * 16f * 0.5f;
            ecoRegionsBounds = default(Bounds);
            ecoRegionsBounds.center = new Vector3(0f, 100 - num2, 0f);
            ecoRegionsBounds.extents = new Vector3(num, num2, num);

            if (ecoRegionsBounds.Contains(pos))
            {
                result.x = (int)((pos.x - ecoRegionsBounds.min.x) / 16f);
                result.y = (int)((pos.y - ecoRegionsBounds.min.y) / 16f);
                result.z = (int)((pos.z - ecoRegionsBounds.min.z) / 16f);
                result = result.Clamp(Int3.zero, new Int3(255, 127, 255));
            }
            else
            {
                result = Int3.zero;
            }
            return result;
        }
        public static Vector3 GetRegionPosition(Int3 index)
        {
            Bounds ecoRegionsBounds;
            float num = 256f * 16f * 0.5f;
            float num2 = 128f * 16f * 0.5f;
            ecoRegionsBounds = default(Bounds);
            ecoRegionsBounds.center = new Vector3(0f, 100 - num2, 0f);
            ecoRegionsBounds.extents = new Vector3(num, num2, num);
            return (new Vector3((float)index.x, (float)index.y, (float)index.z) * 16f + ecoRegionsBounds.min);
        }
        public static float GetRegionMaxDepth(Int3 region)
        {
            int greatestDepthInThisRegionCylinder = depth_dictionary[new Tuple<int, int>(region.x, region.z)];
            Int3 deepestRegionInThisRegionCylinder = new Int3(region.x, greatestDepthInThisRegionCylinder, region.z);
            return GetRegionPosition(deepestRegionInThisRegionCylinder).y;
        }
        public static bool IsThisToothLost(Vector3 pos)
        {
            // The tooth is called "lost" if it's sunk more than 50 meters beyond the deepest ecoregion in this xz-cylinder,
            // as given by the supplied depth dictionary
            // Warning: this is only as good as the depth dictionary we've got
            float minDepth = -50 + GetRegionMaxDepth(GetEcoRegion(pos));
            float thisDepth = pos.y;
            return (thisDepth < minDepth);
        }
        public static float GetMaxDepthHere(Vector3 pos)
        {
            return GetRegionMaxDepth(GetEcoRegion(pos));
        }
    }
}