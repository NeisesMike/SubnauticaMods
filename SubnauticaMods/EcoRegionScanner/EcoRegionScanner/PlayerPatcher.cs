using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Reflection;


namespace EcoRegionScanner
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake")]
    public class PlayerAwakePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            // init the dictionary with a "ceiling value" of 121
            EcoRegionScanner.depthDictionary = new Dictionary<Tuple<int, int>, int>();
            for (int x = 0; x < 256; x++)
            {
                for (int z = 0; z < 256; z++)
                {
                    EcoRegionScanner.depthDictionary.Add(new Tuple<int, int>(x, z), 121);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    public class PlayerUpdatePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (EcoRegionScannerPatcher.config.isScannerActive)
            {
                // get the current ecoregion coordinates as Int3
                Int3 thisEcoRegion = EcoRegionScanner.getEcoRegion(Player.main.transform.position);
                Tuple<int, int> thisLoc = new Tuple<int, int>(thisEcoRegion.x, thisEcoRegion.z);

                // update y to the lesser of the old and the new value
                EcoRegionScanner.depthDictionary[thisLoc] = Math.Min(thisEcoRegion.y, EcoRegionScanner.depthDictionary[thisLoc]);
            }

            // on keyboard input, output the dictionary to file
            if(Input.GetKey(EcoRegionScannerPatcher.config.printMapKey))
            {
                string[] dictStringArray = new string[1];
                dictStringArray[0] = string.Join(Environment.NewLine, EcoRegionScanner.depthDictionary);

                string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                File.WriteAllLines(Path.Combine(modPath, "DepthDictionary.txt"), dictStringArray);
            }

            if(EcoRegionScannerPatcher.config.isFastSeamoth)
            {
                if(Player.main.GetVehicle() && Player.main.GetVehicle().controlSheme == Vehicle.ControlSheme.Submersible)
                {
                    Player.main.GetVehicle().forwardForce = 100f;
                }
            }
            else
            {
                if (Player.main.GetVehicle() && Player.main.GetVehicle().controlSheme == Vehicle.ControlSheme.Submersible)
                {
                    Player.main.GetVehicle().forwardForce = 13f;
                }
            }
        }
    }
}
