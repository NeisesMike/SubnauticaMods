using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BZCommon.Helpers
{
    public class ObjectHelper
    {
        public T GetObjectClone<T>(T uEobject) where T : Object
        {
            return Object.Instantiate(uEobject);
        }

        public T GetComponentClone<T>(T uEcomponent, Transform newParent) where T : Component
        {
            T clone = Object.Instantiate(uEcomponent);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            return clone;
        }

        public GameObject FindDeepChild(Transform parent, string childName)
        {
            Queue<Transform> queue = new Queue<Transform>();

            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                if (c.name == childName)
                {
                    return c.gameObject;
                }

                foreach (Transform t in c)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        public GameObject FindDeepChild(GameObject parent, string childName)
        {
            Queue<Transform> queue = new Queue<Transform>();

            queue.Enqueue(parent.transform);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                if (c.name == childName)
                {
                    return c.gameObject;
                }

                foreach (Transform t in c)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }
        
        public GameObject GetRootGameObject(string sceneName, string startsWith)
        {
            Scene scene;

            GameObject[] rootObjects;

            try
            {
                scene = SceneManager.GetSceneByName(sceneName);
            }
            catch
            {
                return null;
            }

            rootObjects = scene.GetRootGameObjects();

            foreach (GameObject gameObject in rootObjects)
            {
                if (gameObject.name.StartsWith(startsWith))
                {
                    return gameObject;
                }
            }
            return null;
        }
               
        public GameObject GetPrefabClone(GameObject prefab)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = Object.Instantiate(prefab);
            Utils.ZeroTransform(clone.transform);
            return clone;
        }

        public GameObject GetPrefabClone(GameObject prefab, Transform newParent)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = Object.Instantiate(prefab);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);
            return clone;
        }

        public GameObject GetPrefabClone(GameObject prefab, Transform newParent, bool setActive)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = Object.Instantiate(prefab);
            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);
            return clone;
        }

        public void GetPrefabClone(ref GameObject prefab, Transform newParent, bool setActive, string newName, out GameObject clone)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            clone = Object.Instantiate(prefab);
            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            clone.name = newName;
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);            
        }

        public IEnumerator GetModelCloneFromPrefabAsync(TechType techType, string model, Transform newParent, bool setActive, string newName, IOut<GameObject> result)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject clone = null;

            try
            {
                clone = UnityEngine.Object.Instantiate(FindDeepChild(task.GetResult(), model));
            }
            catch
            {
                result.Set(null);
                yield break;
            }

            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            clone.name = newName;
            Utils.ZeroTransform(clone.transform);
            result.Set(clone);
            yield break;
        }

        public GameObject CreateGameObject(string name)
        {
            GameObject newObject = new GameObject(name);            
            Utils.ZeroTransform(newObject.transform);
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            Utils.ZeroTransform(newObject.transform);
            return newObject;
        }

        public GameObject CreateGameObject(PrimitiveType primitiveType, string name, Transform parent)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent, Vector3 localPos)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            Utils.ZeroTransform(newObject.transform);
            newObject.transform.localPosition = localPos;
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, PrimitiveType primitiveType)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot, Vector3 localScale)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            newObject.transform.localScale = localScale;
            return newObject;
        }

        public GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot, PrimitiveType primitiveType)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            return newObject;
        }
    }
}
