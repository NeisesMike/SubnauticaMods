using System;
using System.Collections;
using UnityEngine;

namespace BZCommon.Helpers.Testing
{
    public static class PrefabHelper
    {
        public static GameObject __TESTOBJECT__ = null;

        public static IEnumerator CreateMainMenuTestObjectAsync(TechType techType)
        {
            TaskResult<GameObject> prefabResult = new TaskResult<GameObject>();

            yield return CraftData.InstantiateFromPrefabAsync(techType, prefabResult, false);

            GameObject clone = prefabResult.Get();

            if (__TESTOBJECT__ == null)
            {
                __TESTOBJECT__ = new GameObject("__TESTOBJECT__");

                __TESTOBJECT__.SetActive(false);

                __TESTOBJECT__.AddComponent<PrefabTest>();
            }            

            clone.transform.SetParent(__TESTOBJECT__.transform);

            clone.GetComponent<Rigidbody>().isKinematic = true;

            clone.transform.position = new Vector3(-0.40f, 2.20f, 2.80f);

            clone.transform.rotation = Quaternion.Euler(0, 90, 0);

            try
            {
                SkyApplier skyApplier = clone.GetComponent<SkyApplier>();

                UnityEngine.Object.Destroy(skyApplier);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            foreach (Component component in clone.GetComponents<MonoBehaviour>())
            {
                Type componentType = component.GetType();

                if (componentType == typeof(Rigidbody))
                {
                    continue;
                }
                if (componentType == typeof(WorldForces))
                {
                    continue;
                }
                if (componentType == typeof(PrefabTest))
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(component);
            }

            if (clone.TryGetComponent(out SwimBehaviour swimBehaviour))
            {
                UnityEngine.Object.DestroyImmediate(swimBehaviour);
            }

            if (clone.TryGetComponent(out SplineFollowing splineFollowing))
            {
                UnityEngine.Object.DestroyImmediate(splineFollowing);
            }

            if (clone.TryGetComponent(out Locomotion locomotion))
            {
                UnityEngine.Object.DestroyImmediate(locomotion);
            }

            __TESTOBJECT__.SetActive(true);
        }
    }
}
