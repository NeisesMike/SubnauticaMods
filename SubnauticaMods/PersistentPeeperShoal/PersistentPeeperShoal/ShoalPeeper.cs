using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentPeeperShoal
{
    public class ShoalPeeper : PersistentCreatures.PersistentCreatureBehavior
    {
        public PeeperShoal shoal = null;
		private ShoalBehavior currentBehavior = 0;
		private ShoalBehavior lastBehavior = 0;
		private float AngleMemory = 0;
		private float CurrentHeight = 0; // the height offset from the shoal
		private float HeightTarget = 0;
		private float YVelocity = 0;

		public void Awake()
        {
			AngleMemory = UnityEngine.Random.Range(0, 359);
            base.OverridesBehavior = true;
			gameObject.EnsureComponent<SwimBehaviour>();
        }

		public void Start()
		{
			StopAllCoroutines();
			StartCoroutine(PerformBehavior(currentBehavior));
		}

        public override void UpdateBehavior()
        {
			//throw new NotImplementedException();
			currentBehavior = ChooseBehavior();
			if(currentBehavior != lastBehavior)
			{
				StopAllCoroutines();
				StartCoroutine(PerformBehavior(currentBehavior));
				lastBehavior = currentBehavior;
			}
        }

        public ShoalBehavior ChooseBehavior()
        {
            return ShoalBehavior.ShoalInPlace;
        }

		public IEnumerator PerformBehavior(ShoalBehavior behavior)
		{
			switch (behavior)
			{
				case ShoalBehavior.ShoalInPlace:
					while (true)
					{
						ShoalInPlace();
						yield return new WaitForSeconds(PersistentPeeperShoalPatcher.Config.cycleUpdateRate);
					}
				case ShoalBehavior.SchoolRandom:
					while (true)
					{
						yield return null;
					}
				case ShoalBehavior.SchoolToPoint:
					while (true)
					{
						yield return null;
					}
				case ShoalBehavior.EvadePredators:
					while (true)
					{
						yield return null;
					}
				default:
					break;
			}
		}

		public void ShoalInPlace()
		{
			// ensure the peepers look where they're going
			gameObject.GetComponent<SwimBehaviour>().LookForward();

			// smoothdamp the height of the target 
			CurrentHeight = Mathf.SmoothDamp(CurrentHeight, HeightTarget, ref YVelocity, 1.0f);
			if (Mathf.Abs(CurrentHeight - HeightTarget) < 0.4f)
            {
				//if we're pretty close to the target, scatter a new target somewhere in the cyclone
				HeightTarget = UnityEngine.Random.Range(-PersistentPeeperShoalPatcher.Config.cycleHeight / 2, PersistentPeeperShoalPatcher.Config.cycleHeight / 2);
            }

			// slowly take us around the cycle of a custom ellipse
			AngleMemory += PersistentPeeperShoalPatcher.Config.angleIncrement;
			AngleMemory %= 360;
			float a = PersistentPeeperShoalPatcher.Config.a;
			float b = PersistentPeeperShoalPatcher.Config.b;
			Vector3 nextDest = ShoalGeometry.Ellipse(shoal.transform, a, b, AngleMemory);
			nextDest.y = shoal.transform.position.y + CurrentHeight;

			// We want the peeper to be moving at 5 m/s,
			// so we must account for the YVelocity
			float xzVelocity = Mathf.Sqrt(25 - Mathf.Pow(YVelocity, 2));

			// Issue the SwimTo command
			gameObject.GetComponent<SwimBehaviour>().SwimTo(nextDest, xzVelocity);
		}
	}
}
