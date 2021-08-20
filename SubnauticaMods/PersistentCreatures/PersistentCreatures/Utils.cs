using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentCreatures
{
    public static class Utils
    {
		private static List<TechType> BannedTechTypes = new List<TechType>();

		public static bool IsValidRegion(int x, int y, int z)
        {
			return
			(
				0 <= x && x < PersistentCreatureSimulator.x_max &&
				0 <= y && y < PersistentCreatureSimulator.y_max &&
				0 <= z && z < PersistentCreatureSimulator.z_max &&
				!PersistentCreatureSimulator.terrainMap[x,y,z]
			);
		}
		public static Int3 GetRandomStartLocation()
		{
			int x = -1;
			int y = -1;
			int z = -1;
			while (!IsValidRegion(x, y, z))
			{
				x = UnityEngine.Random.Range(0, PersistentCreatureSimulator.x_max);
				y = UnityEngine.Random.Range(0, PersistentCreatureSimulator.y_max);
				z = UnityEngine.Random.Range(0, PersistentCreatureSimulator.z_max);
			}
			return (new Int3(x, y, z));
		}

		public static float GetRegionDistanceToPlayer(Int3 region)
        {
			return Vector3.Distance(Player.main.transform.position, GetVectorFromRegion(region));
		}

		// The following two methods are implemented based on EcoRegions
		public static Int3 GetRegionFromVector(Vector3 pos)
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
		public static Vector3 GetVectorFromRegion(Int3 region)
		{
			Bounds ecoRegionsBounds;
			float num = 256f * 16f * 0.5f;
			float num2 = 128f * 16f * 0.5f;
			ecoRegionsBounds = default(Bounds);
			ecoRegionsBounds.center = new Vector3(0f, 100 - num2, 0f);
			ecoRegionsBounds.extents = new Vector3(num, num2, num);
			return (new Vector3((float)region.x, (float)region.y, (float)region.z) * 16f + ecoRegionsBounds.min);
		}

		public static void getTerrainMapFromFile()
		{
			Logger.Log("Implement me!");
			// for now, mark only near-surface water as open
			bool[,,] thisTerrainMap = new bool[PersistentCreatureSimulator.x_max, PersistentCreatureSimulator.y_max, PersistentCreatureSimulator.z_max];
			for (int x = 0; x < PersistentCreatureSimulator.x_max; x++)
			{
				for (int z = 0; z < PersistentCreatureSimulator.z_max; z++)
				{
					for (int y = 0; y < 121; y++)
					{
						thisTerrainMap[x, y, z] = true;
					}
					thisTerrainMap[x, 121, z] = false;
					for (int y = 122; y < PersistentCreatureSimulator.y_max; y++)
					{
						thisTerrainMap[x, y, z] = true;
					}
				}
			}
			PersistentCreatureSimulator.terrainMap = thisTerrainMap;
		}

		public static void BanTechType(TechType tt)
        {
			BannedTechTypes.Add(tt);
		}
		public static List<TechType> GetBannedTechTypes()
        {
			return BannedTechTypes;
        }
		public static bool GetIsBanned(TechType tt)
        {
			return BannedTechTypes.Contains(tt);
		}
    }
}
