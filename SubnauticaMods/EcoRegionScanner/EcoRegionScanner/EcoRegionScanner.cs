using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace EcoRegionScanner
{
    public static class EcoRegionScanner
    {
        public static Dictionary<Tuple<int, int>, int> depthDictionary;

        public  static Int3 getEcoRegion(Vector3 pos)
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
    }
}
