using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentCreatures
{
	public abstract class PersistentCreature
	{
		public int unique_id;
		protected GameObject prefab;
		protected GameObject instance;
		protected bool isActiveInWorld;
		protected Tuple<int, int, int> lastLocation;
		protected Tuple<int, int, int> currentLocation;
		private int currentBehavior = -1;

		public void Register(int x, int y, int z)
        {
			PersistentCreatureSimulator.RegisterCreature(this, x, y, z);
        }
		// This will be called by the PersistentCreatureSimulator
		public void SimulatedUpdate(bool[,,] inputMap)
		{
			if (isActiveInWorld)
			{
				if (225 < Vector3.Distance(Player.main.transform.position, instance.transform.position))
				{
					DespawnMe();
					isActiveInWorld = false;
				}
			}
			else
			{
				if (PersistentCreatureSimulator.GetRegionDistanceToPlayer(currentLocation) < 175)
				{
					SpawnMe();
					isActiveInWorld = true;
				}
			}
			if (!isActiveInWorld)
			{
				lastLocation = currentLocation;
				if (currentBehavior == -1)
				{
					currentBehavior = ChooseBehavior(inputMap);
					if (startPerforming(currentBehavior))
					{
						currentBehavior = -1;
					}
				}
				else
				{
					if (continuePerforming(currentBehavior))
					{
						currentBehavior = -1;
					}
				}
				PersistentCreatureSimulator.RemoveFromLocation(this, lastLocation.Item1, lastLocation.Item2, lastLocation.Item3);
				PersistentCreatureSimulator.AddToLocation(this, currentLocation.Item1, currentLocation.Item2, currentLocation.Item3);
			}
		}

		public abstract void SpawnMe();

		public abstract void DespawnMe();

		public abstract int ChooseBehavior(bool[,,] terrainMap);

		// return true if the behavior is complete
		public abstract bool startPerforming(int behavior);

		// return true if the behavior is complete
		public abstract bool continuePerforming(int behavior);
	}
}
