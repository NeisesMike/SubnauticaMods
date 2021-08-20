using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentPeeperShoal
{
	public enum ShoalBehavior
	{
		ShoalInPlace,
		SchoolRandom,
		SchoolToPoint,
		EvadePredators,
	}

	public class PeeperShoal : PersistentCreatures.PersistentCreature
	{
		public float ShoalSpeed = 1;
		private List<GameObject> shoal = new List<GameObject>();
		private Tuple<int, int, int> feedingSpot1;
		private Tuple<int, int, int> feedingSpot2;
		private Tuple<int, int, int> feedingSpot3;
		private ShoalBehavior currentBehavior;

		public override int ChooseBehavior(bool[,,] inputMap)
		{
			return (int)ShoalBehavior.ShoalInPlace;
		}

		public override void SpawnMe()
		{
			if (PersistentPeeperShoalPatcher.Prefab == null)
			{
				PersistentPeeperShoalPatcher.Prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Peeper, true));
				PersistentPeeperShoalPatcher.Prefab.SetActive(false);
			}
			PersistentPeeperShoalPatcher.Prefab.SetActive(true);

			int shoal_size = UnityEngine.Random.Range(PersistentPeeperShoalPatcher.Config.minPeepers, PersistentPeeperShoalPatcher.Config.maxPeepers);
			Vector3 shoalPosition = PersistentCreatures.Utils.GetVectorFromRegion(currentLocation) + UnityEngine.Random.insideUnitSphere * 10f;
			for (int i = 0; i < shoal_size; i++)
			{
				Vector3 scatter = UnityEngine.Random.insideUnitSphere * 1.0f;
				GameObject peeper = GameObject.Instantiate(PersistentPeeperShoalPatcher.Prefab, shoalPosition + scatter, Quaternion.identity);
				peeper.name = "ShoalPeeper";
				ShoalPeeper sp = peeper.AddComponent<ShoalPeeper>();
				sp.shoal = this;
				shoal.Add(peeper);
			}
			PersistentPeeperShoalPatcher.Prefab.SetActive(false);
			Logger.Log("Locations:\nShoal Vector: " + shoalPosition.ToString() + "\nShoal Region: " + currentLocation.ToString() +  "\nPlayer: " + Player.main.transform.position.ToString() + "\nDist: " + Vector3.Distance(shoalPosition, Player.main.transform.position));
		}

		public override void DespawnMe()
		{
			foreach (GameObject go in shoal)
			{
				GameObject.Destroy(go);
			}
			shoal.Clear();
		}

		public override bool startPerforming(int behavior)
		{
			switch ((ShoalBehavior)behavior)
			{
				case ShoalBehavior.ShoalInPlace:
					break;
				case ShoalBehavior.SchoolRandom:
					break;
				case ShoalBehavior.SchoolToPoint:
					break;
				case ShoalBehavior.EvadePredators:
					break;
				default:
					break;
			}
			return true;
		}

		public override bool continuePerforming(int behavior)
		{
			switch ((ShoalBehavior)behavior)
			{
				case ShoalBehavior.ShoalInPlace:
					break;
				case ShoalBehavior.SchoolRandom:
					break;
				case ShoalBehavior.SchoolToPoint:
					break;
				case ShoalBehavior.EvadePredators:
					break;
				default:
					break;
			}
			return true;
		}
	}
}
