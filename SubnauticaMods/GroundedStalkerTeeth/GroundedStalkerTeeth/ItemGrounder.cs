using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GroundedItems
{
    public class FixedSizedQueue<T> : Queue<T>
    {
        public int Size { get; private set; }
        public FixedSizedQueue(int size)
        {
            Size = size;
        }
        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            while (base.Count > Size)
            {
                base.Dequeue();
            }
        }
    }

    public class ItemGrounder : MonoBehaviour
    {
        public void OnEnable()
        {
            StartCoroutine(CheckSettled());
            StartCoroutine(CheckLost());
        }
        public IEnumerator CheckSettled()
        {
            // register our location every 0.25 seconds
            // if we register the same location 10 times in a row,
            // then we'll make ourself kinematic,
            // and stop checking

            FixedSizedQueue<Vector3> lastKnownLocations = new FixedSizedQueue<Vector3>(10);
            while (true)
            {
                // 1. Add our current position to the list of the 10 most recent positions.
                lastKnownLocations.Enqueue(transform.position);

                // 2. If our list is full, check whether they're all the same position. If true, we've settled.
                bool haveWeSettled = false;
                if (lastKnownLocations.Count == 10)
                {
                    haveWeSettled = true;
                    Vector3 potentialSettlePoint = transform.position;
                    foreach (Vector3 thisLoc in lastKnownLocations)
                    {
                        if (thisLoc != potentialSettlePoint)
                        {
                            haveWeSettled = false;
                            break;
                        }
                    }
                }

                // 3. If we haven't settled, try again later. Otherwise, quit.
                if (haveWeSettled)
                {
                    // if we've settled, then let's make sure we don't move ever again.
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    StopAllCoroutines();
                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }
        public IEnumerator CheckLost()
        {
            while (true)
            {
                // I thought we should wait 10 seconds, so we wait 20 instead
                yield return new WaitForSeconds(20f);
                // if we're below the maximum feasible depth, send us to the minimum feasible depth
                if (DepthManager.IsThisToothLost(transform.position))
                {
                    // We add 20 here as a measure to ensure we end up above the terrain
                    float newDepth = DepthManager.GetMaxDepthHere(transform.position);
                    transform.position = new Vector3(transform.position.x, newDepth, transform.position.z);
                }
            }
        }
    }
}
