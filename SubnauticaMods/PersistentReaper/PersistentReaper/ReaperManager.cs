using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace PersistentReaper
{
    public static class ReaperManager
    {
        public static GameObject reaperParent = new GameObject("Reaper Parent Object");
        public static Dictionary<GameObject, ReaperBehavior> reaperDict = new Dictionary<GameObject, ReaperBehavior>();
        public static Dictionary<Int3, EcoRegion> ecoRegionDict;
        public static Dictionary<Int3, Scent> playerTrailDict = new Dictionary<Int3, Scent>();
        private static System.Random manRand = new System.Random();
        private static int spawnRadius = 225;
        private static bool areAllReapersDespawned = false;
        // grab reaper prefab
        private static GameObject reaperPrefab = CraftData.GetPrefabForTechType(TechType.ReaperLeviathan, true);

        // updateInterval is measured in seconds
        private static float updateInterval = 1f;
        private static float lastUpdateTime = Time.time;

        // depthDictionary is the set of legal volumes which apparently have "fallen to rest"
        // That is, for a given (x,z) key, the y value represents the lowest legal plane for the percies
        public static Dictionary<Tuple<int, int>, int> depthDictionary;
        private static bool areDictionariesBuilt = false;

        public static void initReaperManager()
        {
            if (!areDictionariesBuilt)
            {
                depthDictionary = getDepthDictionary();
                for (int x = 0; x < 256; x++)
                {
                    for (int y = 0; y < 256; y++)
                    {
                        for (int z = 0; z < 256; z++)
                        {
                            Int3 thisLoc = new Int3(x, y, z);
                            playerTrailDict.Add(thisLoc, null);
                        }
                    }
                }
                startReapers();
                areDictionariesBuilt = true;
            }
        }

        public static void startReapers()
        {
            for (int i = 0; i < PersistentReaperPatcher.Config.numReapers; i++)
            {
                initOneReaper();
            }
            return;
        }

        public static void initOneReaper()
        {
            // instantiate Percy somewhere up in the air
            Vector3 spawnLocation = new Vector3(0, -300, 0);
            GameObject Percy = UnityEngine.Object.Instantiate(reaperPrefab, spawnLocation, Quaternion.identity);

            if(!reaperParent)
            {
                reaperParent = new GameObject("Reaper Parent Object");
            }
            Percy.transform.parent = reaperParent.transform;
            ReaperBehavior percyBehavior = new ReaperBehavior();

            // ensure Percy will wander freely
            Percy.GetComponent<SwimRandom>().swimRadius = new Vector3(100f, 20f, 100f);
            Percy.GetComponent<StayAtLeashPosition>().leashDistance = float.MaxValue;

            // make Percy able to handle a punch or two
            // remember, Percy has 5000 health
            if (PersistentReaperPatcher.Config.reaperBehaviors == ReaperBehaviors.Bloodthirsty || PersistentReaperPatcher.Config.reaperBehaviors == ReaperBehaviors.HumanHunter)
            {
                Percy.GetComponent<FleeOnDamage>().damageThreshold = 1000f;
            }

            // place Percy in a "random" EcoRegion
            Int3 regionIndex = getRandomRegion();
            if (regionIndex != Int3.negativeOne)
            {
                percyBehavior.currentRegion = regionIndex;
            }
            else
            {
                percyBehavior.currentRegion = new Int3(0, 0, 0);
            }

            // store this Percy in the reaperList
            reaperDict.Add(Percy, percyBehavior);

            // deactivate Percy
            Percy.SetActive(false);
        }

        public static void updateReapers()
        {
            if (PersistentReaperPatcher.Config.areReapersActive)
            {
                if(reaperDict.Count != PersistentReaperPatcher.Config.numReapers)
                {
                    int difference = reaperDict.Count - PersistentReaperPatcher.Config.numReapers;
                    if(difference > 0)
                    {
                        for(int i=0; i<difference; i++)
                        {
                            UnityEngine.Object.Destroy(reaperDict.Keys.First().gameObject);
                            reaperDict.Remove(reaperDict.Keys.First());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < -difference; i++)
                        {
                            initOneReaper();
                        }
                    }
                }

                areAllReapersDespawned = false;
                if (lastUpdateTime + updateInterval < Time.time)
                {
                    foreach (KeyValuePair<GameObject, ReaperBehavior> entry in reaperDict)
                    {
                        if(!entry.Key)
                        {
                            continue;
                        }

                        // if percy is not active,
                        // move him to an adjacent valid region
                        if (!entry.Key.activeSelf)
                        {
                            if (PersistentReaperPatcher.Config.reaperBehaviors == ReaperBehaviors.HumanHunter)
                            {
                                moveWithScent(entry.Value);
                            }
                            else
                            {
                                randomMove(entry.Value);
                            }
                        }

                        // let's see if we're in range to spawn,
                        // or if we're so far away we should despawn
                        controlSpawning(entry.Key);

                        lastUpdateTime = Time.time;
                    }
                }
            }
            else if(!areAllReapersDespawned)
            {
                despawnAllReapers();
            }
        }

        private static void despawnAllReapers()
        {
            foreach (KeyValuePair<GameObject, ReaperBehavior> entry in reaperDict)
            {
                if (entry.Key)
                {
                    entry.Key.SetActive(false);
                }
            }
            areAllReapersDespawned = true;
        }

        private static void randomMove(ReaperBehavior percy)
        {
            for (int fuel = 100; 0 < fuel; fuel--)
            {
                Int3 currentLoc = percy.currentRegion;

                // move somewhere in the cube...
                // maybe even stay still
                int direction = manRand.Next(27);
                currentLoc.x += (direction % 3) - 1;
                currentLoc.y += ((direction % 9) / 3) - 1;
                currentLoc.z += ((direction % 27) / 9) - 1;

                if (checkRegionLegality(currentLoc))
                {
                    percy.currentRegion = currentLoc;
                    return;
                }
            }
        }
        private static void moveWithScent(ReaperBehavior percy)
        {
            // build list of legal moves
            List<Int3> moveList = new List<Int3>();
            for (int i = 0; i < 27; i++)
            {
                Int3 thisLoc = percy.currentRegion;
                thisLoc.x += (i % 3);
                thisLoc.y += (i % 9) / 3;
                thisLoc.z += (i % 27) / 9;
                if (checkRegionLegality(thisLoc))
                {
                    moveList.Add(thisLoc);
                }
            }

            // choose the destination with the highest scent intensity
            int maxScentIntensity = 0;
            foreach (Int3 move in moveList)
            {
                if(playerTrailDict[move] == null)
                {
                    continue;
                }
                if (maxScentIntensity < playerTrailDict[move].scentIntensity)
                {
                    maxScentIntensity = playerTrailDict[move].scentIntensity;
                }
            }

            if (maxScentIntensity == 0)
            {
                randomMove(percy);
            }
            else
            {
                moveList = moveList.FindAll(e => getScentIntensity(e) == maxScentIntensity);
                percy.currentRegion = moveList[manRand.Next(moveList.Count)];
            }
        }

        private static int getScentIntensity(Int3 loc)
        {
            if (playerTrailDict[loc] == null)
            {
                return 0;
            }
            else
            {
                return playerTrailDict[loc].scentIntensity;
            }
        }

        public static Vector3 tryMoveToScent(Vector3 startLocation)
        {
            // build list of legal moves
            List<Int3> moveList = new List<Int3>();
            for (int i = 0; i < 27; i++)
            {
                Int3 thisLoc = getEcoRegion(startLocation);
                thisLoc.x += (i % 3);
                thisLoc.y += (i % 9) / 3;
                thisLoc.z += (i % 27) / 9;
                if (checkRegionLegality(thisLoc))
                {
                    moveList.Add(thisLoc);
                }
            }

            // choose the destination with the highest scent intensity
            int maxScentIntensity = 0;
            foreach (Int3 move in moveList)
            {
                if (playerTrailDict[move] == null)
                {
                    continue;
                }
                if (maxScentIntensity < playerTrailDict[move].scentIntensity)
                {
                    maxScentIntensity = playerTrailDict[move].scentIntensity;
                }
            }

            if (maxScentIntensity == 0)
            {
                return Vector3.zero;
            }
            else
            {
                moveList = moveList.FindAll(e => getScentIntensity(e) == maxScentIntensity);
                return (getRegionPosition(moveList[manRand.Next(moveList.Count)]));
            }
        }

        private static void controlSpawning(GameObject percy)
        {
            // check whether we need to spawn or despawn Percy
            bool isPercyWithinRange = isWithinRange(percy);
            bool isPercyActive = percy.activeSelf;
            if (isPercyWithinRange && !isPercyActive)
            {
                // activate Percy
                percy.transform.position = getRegionPosition(reaperDict[percy].currentRegion);
                percy.SetActive(true);
            }
            else if (!isPercyWithinRange && isPercyActive)
            {
                // deactivate Percy
                percy.SetActive(false);

                // lock off
                reaperDict[percy].isLockedOntoPlayer = false;

                // update Percy's location
                reaperDict[percy].currentRegion = getEcoRegion(percy.transform.position);
            }
        }

        public static Int3 getEcoRegion(Vector3 pos)
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
        private static bool isWithinRange(GameObject percy)
        {
            float dist = Vector3.Distance(Player.main.transform.position, percy.transform.position);
            if (percy.activeSelf && dist < spawnRadius)
            {
                return true;
            }

            dist = Vector3.Distance(getRegionPosition(reaperDict[percy].currentRegion), Player.main.transform.position);
            if (!percy.activeSelf && dist < spawnRadius)
            {
                return true;
            }

            return false;
        }

        public static Vector3 getRegionPosition(Int3 index)
        {
            Bounds ecoRegionsBounds;
            float num = 256f * 16f * 0.5f;
            float num2 = 128f * 16f * 0.5f;
            ecoRegionsBounds = default(Bounds);
            ecoRegionsBounds.center = new Vector3(0f, 100 - num2, 0f);
            ecoRegionsBounds.extents = new Vector3(num, num2, num);
            return (new Vector3((float)index.x, (float)index.y, (float)index.z) * 16f + ecoRegionsBounds.min);
        }

        // the legal volume should range from sea level down to the highest terrain point in the region
        private static bool checkRegionLegality(Int3 index)
        {
            bool isValid = 0 <= index.x && index.x < 256 && index.y <= 121 && 0 <= index.z && index.z < 256;
            if (!isValid)
            {
                return false;
            }

            Tuple<int, int> thisLocation = new Tuple<int, int>(index.x, index.z);
            if(depthDictionary[thisLocation] != -1 && depthDictionary[thisLocation] <= index.y)
            {
                return true;
            }

            return false;
        }

        private static Int3 getRandomRegion()
        {
            for(int fuel=100; 0<fuel; fuel--)
            {
                int x = manRand.Next(256);
                int z = manRand.Next(256);
                int minY = depthDictionary[new Tuple<int, int>(x, z)];
                int y = minY + manRand.Next(121 - minY);
                Int3 thisValue = new Int3(x, y, z);
                if(checkRegionLegality(thisValue))
                {
                    return thisValue;
                }
            }
            return Int3.negativeOne;
        }

        public static Dictionary<Tuple<int, int>, int> getDepthDictionary()
        {
            Dictionary<Tuple<int, int>, int> depthDictionary = new Dictionary<Tuple<int, int>, int>();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string depthMapString = "";
            switch(PersistentReaperPatcher.Config.depthMapChoice)
            {
                case DepthMap.Normal:
                    depthMapString = "DepthDictionary.txt";
                    break;
                case DepthMap.NoShallowReapers:
                    depthMapString = "DepthDictionary_NoShallowReapers.txt";
                    break;
            }

            string[] dictStringArr = File.ReadAllLines(Path.Combine(modPath, depthMapString));

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
