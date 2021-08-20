using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentCreatures
{
	public abstract class PersistentCreature : MonoBehaviour
	{
		public PersistentCreatureBehavior behavior = null;

		public int unique_id;
		protected bool isActiveInWorld;
		protected Int3 lastLocation;
		protected Int3 currentLocation;
		private int currentBehavior = -1;
		protected float distToSpawn = 50f;
		protected float distToDespawn = 225f;

		public void Register(int x, int y, int z)
		{
			currentLocation = new Int3(x, y, z);
			PersistentCreatureSimulator.RegisterCreature(this, x, y, z);
        }
		// This will be called by the PersistentCreatureSimulator
		public void SimulatedUpdate(bool[,,] inputMap)
		{
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
				PersistentCreatureSimulator.RemoveFromLocation(this, lastLocation.x, lastLocation.y, lastLocation.z);
				PersistentCreatureSimulator.AddToLocation(this, currentLocation.x, currentLocation.y, currentLocation.z);
			}
		}

		public void ControlSpawning()
		{
			if (isActiveInWorld)
			{
				if (distToDespawn < Vector3.Distance(Player.main.transform.position, Utils.GetVectorFromRegion(currentLocation)))
				{
					SpawnMe();
					isActiveInWorld = false;
				}
			}
			else
			{
				if (Utils.GetRegionDistanceToPlayer(currentLocation) < distToSpawn)
				{
					DespawnMe();
					isActiveInWorld = true;
				}
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
