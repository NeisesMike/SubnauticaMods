using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace InsertGameObject
{
    class GameObjectInserter : MonoBehaviour
    {
        private static GameObject prefab = null;
        public void Start()
        {
            GetTestPrefab();
        }
        public void Update()
        {
            if (Input.GetKeyDown(MainPatcher.config.SpawnKey))
            {
                if(prefab is null)
                {
                    Logger.Warn("prefab was null. getting again");
                    GetTestPrefab();
                }
                GameObject thisGobj = Utils.SpawnFromPrefab(prefab, null);
                thisGobj.transform.position = Player.main.transform.position + Player.main.camRoot.transform.forward * MainPatcher.config.SpawnDistance;
                thisGobj.transform.rotation = Quaternion.identity;
                thisGobj.transform.localScale *= MainPatcher.config.SizeMultiplier;
            }
        }
        public static bool GetTestPrefab()
        {
            if (prefab is null)
            {
                // load the asset bundle
                string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "insert_bundle"));
                if (myLoadedAssetBundle == null)
                {
                    Logger.Log("Failed to load AssetBundle!");
                    return false;
                }

                System.Object[] arr = myLoadedAssetBundle.LoadAllAssets();
                foreach (System.Object obj in arr)
                {
                    if (obj.ToString().Contains("test_model"))
                    {
                        prefab = (GameObject)obj;
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
