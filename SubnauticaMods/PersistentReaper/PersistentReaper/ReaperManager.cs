﻿using System;
using System.Collections.Generic;
using System.Collections;
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
        /*
         * The Reaper legal y-range is [85,121] in the normal map.
         * maybe we can optimize the dictionaries?
         */
        public static int minYReaperDepth = 80;
        public static int maxYReaperDepth = 121;
        public static GameObject reaperParent = new GameObject("Reaper Parent Object");
        /* reaperDict
         * contains PersistentReaperPatcher.config.numReapers many ReaperBehaviors
         * ReaperBehaviours Have their locations simulated
         * When a reaper behavior is near enough to the player,
         * a reaper game object is spawned and associated with it.
         */
        public static Dictionary<ReaperBehavior, GameObject> reaperDict = new Dictionary<ReaperBehavior, GameObject>();
        public static Dictionary<Int3, Scent> playerTrailDict = new Dictionary<Int3, Scent>();
        private readonly static System.Random ManRand = new System.Random();
        private const int SpawnRadius = 225;
        public static GameObject ReaperPrefab { get; set; }

        // updateInterval is measured in seconds
        private static float lastUpdateTime = Time.time;

        // depthDictionary is the set of legal volumes which apparently have "fallen to rest"
        // That is, for a given (x,z) key, the y value represents the lowest legal plane for the percies
        public static Dictionary<Tuple<int, int>, int> depthDictionary;
        private static bool areDictionariesBuilt = false;

        public static Dictionary<Int3, Scent> GetScentDictionary()
        {
            Dictionary<Int3, Scent> thisScentDict = new Dictionary<Int3, Scent>();
            for (int x = 0; x < 256; x++)
            {
                // limit the y-range of the scent dictionary to save a bit of space
                // unfortunately this means we need to always check before we access the scent dictionary
                for (int y = minYReaperDepth; y < maxYReaperDepth; y++)
                {
                    for (int z = 0; z < 256; z++)
                    {
                        Int3 thisLoc = new Int3(x, y, z);
                        thisScentDict.Add(thisLoc, null);
                    }
                }
            }
            return thisScentDict;
        }

        // init and remove control the existence of a reaperbehavior
        public static void InitOneReaper()
        {
            ReaperBehavior percyBehavior = new ReaperBehavior();
            // place Percy in a "random" EcoRegion
            Int3 regionIndex = GetRandomRegion();
            if (regionIndex != Int3.negativeOne)
            {
                percyBehavior.currentRegion = regionIndex;
            }
            else
            {
                percyBehavior.currentRegion = new Int3(0, 0, 0);
            }
            // store this Percy in the reaperList
            reaperDict.Add(percyBehavior, null);
        }
        public static void RemoveOneReaper()
        {
            if (reaperDict[reaperDict.Keys.First()])
            {
                UnityEngine.Object.Destroy(reaperDict[reaperDict.Keys.First()]);
            }
            reaperDict.Remove(reaperDict.Keys.First());
        }
        // spawn and despawn control the existence of a game object attached to a Reaper Behavior
        public static void DespawnThisReaper(ReaperBehavior percy)
        {
            if (reaperDict[percy])
            {
                UnityEngine.Object.Destroy(reaperDict[percy]);
            }
            reaperDict[percy] = null;
        }
        public static void SpawnThisReaper(ReaperBehavior thisReaper)
        {
            Vector3 spawnLocation = GetRegionPosition(thisReaper.currentRegion);
            if(IsIllegalPosition(spawnLocation))
            {
                return;
            }
            GameObject Percy = UnityEngine.Object.Instantiate(ReaperPrefab, spawnLocation, Quaternion.identity);
            if (!reaperParent)
            {
                reaperParent = new GameObject("Reaper Parent Object");
            }
            Percy.transform.parent = reaperParent.transform;
            // ensure Percy will wander freely
            Percy.GetComponent<SwimRandom>().swimRadius = new Vector3(100f, 20f, 100f);
            Percy.GetComponent<StayAtLeashPosition>().leashDistance = float.MaxValue;
            // make Percy able to handle a punch or two
            // remember, Percy has 5000 health
            if (PersistentReaperPatcher.PRConfig.reaperBehaviors == ReaperBehaviors.Bloodthirsty || PersistentReaperPatcher.PRConfig.reaperBehaviors == ReaperBehaviors.HumanHunter)
            {
                Percy.GetComponent<FleeOnDamage>().damageThreshold = 1000f;
            }
            // ensure Percy is not saved
            Component.DestroyImmediate(Percy.GetComponent<PrefabIdentifier>());
            // attach this gameobject to percy
            reaperDict[thisReaper] = Percy;
        }
        public static void UpdateReapers()
        {
            // ensure these dictionaries are built,
            // and build them only once
            if (!areDictionariesBuilt)
            {
                depthDictionary = GetDepthDictionary();
                playerTrailDict = GetScentDictionary();
                areDictionariesBuilt = true;
            }

            // adjust the number of available reapers as necessary
            ManagerReaperCount();

            // PersistentReaper.Update
            if (lastUpdateTime + PersistentReaperPatcher.PRConfig.updateInterval < Time.time)
            {
                lastUpdateTime = Time.time;
                try
                {
                    SimulateReaperBehaviors();
                }
                catch (InvalidOperationException exception)
                {
                    // if we've slid the numReapers slider -while- we're in this foreach,
                    // just die gracefully,
                    // and try again later
                    return;
                }
            }
        }
        public static int GetNumConfigReapers()
        {
            int result = 0;
            result += PersistentReaperPatcher.PRConfig.numThousandReapers * 1000;
            result += PersistentReaperPatcher.PRConfig.numHundredReapers * 100;
            result += PersistentReaperPatcher.PRConfig.numTenReapers * 10;
            result += PersistentReaperPatcher.PRConfig.numSingleReapers * 1;
            return result;
        }
        public static void ManagerReaperCount()
        {
            int numReapers = GetNumConfigReapers();
            while(reaperDict.Count < numReapers)
            {
                InitOneReaper();
            }
            while (reaperDict.Count > numReapers)
            {
                RemoveOneReaper();
            }
        }
        public static void SimulateReaperBehaviors()
        {
            foreach (KeyValuePair<ReaperBehavior, GameObject> entry in reaperDict)
            {
                // if percy has no body,
                // simulate his movement
                if (entry.Value == null)
                {
                    if (PersistentReaperPatcher.PRConfig.reaperBehaviors == ReaperBehaviors.HumanHunter)
                    {
                        MoveWithScent(entry.Key);
                    }
                    else
                    {
                        RandomMove(entry.Key);
                    }
                }
                else
                {
                    // if percy has a body, update his position to where his gameobject is
                    entry.Key.currentRegion = GetEcoRegion(entry.Value.transform.position);
                }
                // let's see if we're in range to spawn,
                // or if we're so far away we should despawn
                ControlSpawning(entry);
            }
        }
        private static void ControlSpawning(KeyValuePair<ReaperBehavior, GameObject> percy)
        {
            // check whether we need to spawn or despawn Percy
            if (percy.Value == null && IsWithinRange(percy.Key) && reaperDict[percy.Key] == null)
            {
                SpawnThisReaper(percy.Key);
            }
            else if (percy.Value != null && !IsWithinRange(percy.Key) && reaperDict[percy.Key] != null)
            {
                DespawnThisReaper(percy.Key);
            }
        }
        private static void RandomMove(ReaperBehavior percy)
        {
            for (int fuel = 100; 0 < fuel; fuel--)
            {
                Int3 currentLoc = percy.currentRegion;

                // move somewhere in the cube...
                // maybe even stay still
                int direction = ManRand.Next(27);
                currentLoc.x += (direction % 3) - 1;
                currentLoc.y += ((direction % 9) / 3) - 1;
                currentLoc.z += ((direction % 27) / 9) - 1;

                if (CheckRegionLegality(currentLoc))
                {
                    percy.currentRegion = currentLoc;
                    return;
                }
            }
        }
        private static void MoveWithScent(ReaperBehavior percy)
        {
            // build list of legal moves
            List<Int3> moveList = new List<Int3>();
            for (int i = 0; i < 27; i++)
            {
                Int3 thisLoc = percy.currentRegion;
                thisLoc.x += (i % 3);
                thisLoc.y += (i % 9) / 3;
                thisLoc.z += (i % 27) / 9;
                if (CheckRegionLegality(thisLoc))
                {
                    moveList.Add(thisLoc);
                }
            }

            // choose the destination with the highest scent intensity
            // in the last step, we've guaranteed these scent locations are legal,
            // so we won't do that again here.
            int maxScentIntensity = 0;
            foreach (Int3 move in moveList)
            {
                int thisScent = GetScentIntensity(move);
                if (maxScentIntensity < thisScent)
                {
                    maxScentIntensity = thisScent;
                }
            }

            if (maxScentIntensity == 0)
            {
                RandomMove(percy);
            }
            else
            {
                moveList = moveList.FindAll(e => GetScentIntensity(e) == maxScentIntensity);
                percy.currentRegion = moveList[ManRand.Next(moveList.Count)];
            }
        }
        private static int GetScentIntensity(Int3 loc)
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
        public static Vector3 TryMoveToScent(Vector3 startLocation)
        {
            // build list of legal moves
            List<Int3> moveList = new List<Int3>();
            for (int i = 0; i < 27; i++)
            {
                Int3 thisLoc = GetEcoRegion(startLocation);
                thisLoc.x += (i % 3);
                thisLoc.y += (i % 9) / 3;
                thisLoc.z += (i % 27) / 9;
                if (CheckRegionLegality(thisLoc))
                {
                    moveList.Add(thisLoc);
                }
            }

            // choose the destination with the highest scent intensity
            int maxScentIntensity = 0;
            foreach (Int3 move in moveList)
            {
                int thisScent = GetScentIntensity(move);
                if (maxScentIntensity < thisScent)
                {
                    maxScentIntensity = thisScent;
                }
            }

            if (maxScentIntensity == 0)
            {
                return Vector3.zero;
            }
            else
            {
                moveList = moveList.FindAll(e => GetScentIntensity(e) == maxScentIntensity);
                return (GetRegionPosition(moveList[ManRand.Next(moveList.Count)]));
            }
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
        private static bool IsWithinRange(ReaperBehavior percy)
        {
            float dist = Vector3.Distance(GetRegionPosition(percy.currentRegion), Player.main.transform.position);
            if (dist < SpawnRadius)
            {
                return true;
            }
            return false;
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
        // the legal volume should range from sea level down to the highest terrain point in the region
        public static bool CheckRegionLegality(Int3 index)
        {
            bool isValid = 0 <= index.x && index.x < 256 && minYReaperDepth <= index.y && index.y < maxYReaperDepth && 0 <= index.z && index.z < 256;
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
        private static Int3 GetRandomRegion()
        {
            for(int fuel=100; 0<fuel; fuel--)
            {
                int x = ManRand.Next(256);
                int z = ManRand.Next(256);
                int minY = depthDictionary[new Tuple<int, int>(x, z)];
                int y = minY + ManRand.Next(121 - minY);
                Int3 thisValue = new Int3(x, y, z);
                if(CheckRegionLegality(thisValue))
                {
                    return thisValue;
                }
            }
            return Int3.negativeOne;
        }
        public static Dictionary<Tuple<int, int>, int> GetDepthDictionary()
        {
            if (PersistentReaperPatcher.PRConfig == null)
            {
                PersistentReaperPatcher.PRLogger.LogWarning("Config not yet available. Will get the depth dictionary later.");
                return new Dictionary<Tuple<int, int>, int>();
            }
            return GetDepthDictionary(PersistentReaperPatcher.PRConfig.depthMapChoice);
        }
        public static Dictionary<Tuple<int, int>, int> GetDepthDictionary(DepthMap depthMapChoice)
        {
            Dictionary<Tuple<int, int>, int> depthDictionary = new Dictionary<Tuple<int, int>, int>();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string depthMapString = "";
            switch (depthMapChoice)
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
                Tuple<int, int, int> thisEntry = GetDepthEntry(entry);
                Tuple<int, int> thisLocation = new Tuple<int, int>(thisEntry.Item1, thisEntry.Item3);
                depthDictionary.Add(thisLocation, thisEntry.Item2);
            }
            return depthDictionary;
        }
        private static Tuple<int, int, int> GetDepthEntry(string entryString)
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
        public static bool IsIllegalPosition(Vector3 location)
        {
            Vector3 startingLocation = new Vector3(-13.3f, 5.8f, 23.6f);
            Vector3 auroraLocation = new Vector3(1027f, 9f, 18.6f);
            //thanks, Xunnamius, for this float value
            float safeRadius = 300f;
            // if within 300m of landing pod 
            return (Vector3.Distance(startingLocation, location) < safeRadius)
                || (Vector3.Distance(auroraLocation, location) < safeRadius);
        }
    }
}
