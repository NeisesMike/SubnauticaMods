using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace PersistentPeeperShoal
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatcher
    {
        /*
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix()
        {
        }
        */

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePostfix()
        {
            DebugTrySpawnShoal();
            DebugAdjustCycle();
        }

        public static void DebugTrySpawnShoal()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                // Ensure we have the peeper prefab
                if (PersistentPeeperShoalPatcher.Prefab == null)
                {
                    PersistentPeeperShoalPatcher.Prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Peeper, true));
                    PersistentPeeperShoalPatcher.Prefab.SetActive(false);
                }

                // Activate the prefab peeper
                PersistentPeeperShoalPatcher.Prefab.SetActive(true);

                // spawn a shoal in front of us
                Vector3 shoalPosition = MainCameraControl.main.transform.position + 15 * MainCameraControl.main.transform.forward;
                GameObject shoalObj = new GameObject("DebugShoal");
                shoalObj.transform.position = shoalPosition;
                shoalObj.AddComponent(typeof(PeeperShoal));
                for (int i = 0; i < PersistentPeeperShoalPatcher.Config.maxPeepers; i++)
                {
                    Vector3 scatter = UnityEngine.Random.insideUnitSphere * 1.0f;
                    GameObject peeper = GameObject.Instantiate(PersistentPeeperShoalPatcher.Prefab, shoalPosition + scatter, Quaternion.identity);
                    ShoalPeeper sp = peeper.AddComponent<ShoalPeeper>();
                    sp.shoal = shoalObj.GetComponent<PeeperShoal>();
                }

                // Deactivate the prefab peeper
                PersistentPeeperShoalPatcher.Prefab.SetActive(false);
            }
        }

        public static void DebugAdjustCycle()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                PersistentPeeperShoalPatcher.Config.angleIncrement += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                PersistentPeeperShoalPatcher.Config.angleIncrement -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                PersistentPeeperShoalPatcher.Config.cycleUpdateRate += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                PersistentPeeperShoalPatcher.Config.cycleUpdateRate -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                PersistentPeeperShoalPatcher.Config.geoScale += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                PersistentPeeperShoalPatcher.Config.geoScale -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                PersistentPeeperShoalPatcher.Config.cycleHeight += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                PersistentPeeperShoalPatcher.Config.cycleHeight -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PersistentPeeperShoalPatcher.Config.a += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PersistentPeeperShoalPatcher.Config.a -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PersistentPeeperShoalPatcher.Config.b += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PersistentPeeperShoalPatcher.Config.b -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                Logger.output("saved!");
                Dictionary<string, float> config = new Dictionary<string, float>();
                config.Add("Angle Increment", PersistentPeeperShoalPatcher.Config.angleIncrement);
                config.Add("Update Interval", PersistentPeeperShoalPatcher.Config.cycleUpdateRate);
                config.Add("Scale", PersistentPeeperShoalPatcher.Config.geoScale);
                config.Add("Height", PersistentPeeperShoalPatcher.Config.cycleHeight);
                config.Add("a", PersistentPeeperShoalPatcher.Config.a);
                config.Add("b", PersistentPeeperShoalPatcher.Config.b);

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CycleConfigs.txt");

                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Bing:");
                    var lines = config.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                    var output = string.Join(Environment.NewLine, lines);
                    sw.WriteLine(output);
                }
            }
        }
    }
}
