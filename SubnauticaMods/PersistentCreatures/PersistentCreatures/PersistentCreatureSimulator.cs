using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace PersistentCreatures
{
	public class PersistentCreatureSimulator : MonoBehaviour
	{
		// a pre-generated Terrain Map
		public static bool[,,] terrainMap;

		// the width, height, and depth of the world in regions
		private static int x_max = 256;
		private static int y_max = 128;
		private static int z_max = 256;

		// a unique-id for each persistent creature
		private static int unique_id = 0;

		// the number of creatures to allocate to each thread
		private static int taskSize = 50;

		// This 3D array of dictionaries supports finding PC neighbors in O(1) time.
		// Each PersistentCreature manages its own value in this array.
		// To correctly find neighbors, it requires PCs to update their currentLocation faithfully.
		private static Dictionary<int, PersistentCreature>[,,] creatureMap = new Dictionary<int, PersistentCreature>[x_max, y_max, z_max];

		// This dictionary ensures O(1) time for adding and removing.
		// It also retrieves .Values in O(1), so iterating over all creatures is O(n)
		private static Dictionary<int, PersistentCreature> creatureList = new Dictionary<int, PersistentCreature>();

		public void Start()
		{
			StartCoroutine(SimulatedUpdate());
		}

		/*
		 * Ultimately this method is O(n*m) where
		 *		n is the number of PCs currently registered
		 *		m is the number of PCs in the region this PC is in
		 */
		private IEnumerator SimulatedUpdate()
		{
			while (true)
			{
				yield return new WaitForSeconds(PersistentCreaturesPatcher.Config.simulationPeriod);
				Logger.Log("Tick");
				int numTasks = 0;
				List<Thread> threads = new List<Thread>();
				while (numTasks * taskSize < PersistentCreatureSimulator.getNumCreatures())
				{
					Thread thisThread = new Thread(Simulate);
					thisThread.Start(numTasks);
					threads.Add(thisThread);
					numTasks++;
				}
			}
		}

		private static void Simulate(object taskID)
		{
			foreach (PersistentCreature pc in creatureList.Values.Skip((int)taskID * taskSize).Take((int)taskID * taskSize))
			{
				pc.SimulatedUpdate(terrainMap);
			}
		}

		// call this at patch time
		public static void Init()
		{
			getTerrainMapFromFile();
			for (int x = 0; x < x_max; x++)
			{
				for (int y = 0; y < y_max; y++)
				{
					for (int z = 0; z < z_max; z++)
					{
						creatureMap[x, y, z] = new Dictionary<int, PersistentCreature>();
					}
				}
			}
		}


		public static int getNumCreatures()
		{
			return creatureList.Count;
		}
		public static Dictionary<int, PersistentCreature>.ValueCollection getCreatures()
        {
			return creatureList.Values;
        }

		public static void RegisterCreature(PersistentCreature pc, int x, int y, int z)
		{
			pc.unique_id = unique_id;
			creatureList.Add(unique_id, pc);
			creatureMap[x, y, z].Add(unique_id, pc);
			unique_id++;
		}
		public static void RemoveCreature(PersistentCreature pc, int x, int y, int z)
		{
			creatureList.Remove(pc.unique_id);
			creatureMap[x, y, z].Remove(pc.unique_id);
		}

		public static void AddToLocation(PersistentCreature pc, int x, int y, int z)
		{
			creatureMap[x, y, z].Add(pc.unique_id, pc);
		}
		public static void RemoveFromLocation(PersistentCreature pc, int x, int y, int z)
		{
			creatureMap[x, y, z].Remove(pc.unique_id);
		}

		// return PCs in a cube around this PC
		public static List<PersistentCreature> GetNeighbors(int inX, int inY, int inZ)
		{
			List<PersistentCreature> neighbors = new List<PersistentCreature>();
			for (int x = inX - 1; x <= inX + 1; x++)
			{
				for (int y = inY - 1; y <= inY + 1; y++)
				{
					for (int z = inZ - 1; z <= inZ + 1; z++)
					{
						if (IsValidRegion(x, y, z))
						{
							neighbors.AddRange(creatureMap[x, y, z].Values);
						}
					}
				}
			}
			return neighbors;
		}

		public static bool IsValidRegion(int x, int y, int z)
        {
			return
			(
				0 <= x && x < x_max &&
				0 <= y && y < y_max &&
				0 <= z && z < z_max
			);
        }

		public static float GetRegionDistanceToPlayer(Tuple<int,int,int> region)
        {
			Logger.Log("Implement me!");
			// somehow convert region to a Vector3
			// and then to Vector3.Distance with Player.main.transform.position
			return 0;
		}

		public static void getTerrainMapFromFile()
		{
			Logger.Log("Implement me!");
		}
	}
}
