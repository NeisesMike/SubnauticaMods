using System.IO;
using System.Reflection;
using UnityEngine;

namespace AttitudeIndicator
{
    internal static class AssetGetter
    {
        internal static GameObject prefab = null;
        internal static void GetAssets()
        {
            // load the asset bundle
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "assets/attitudeindicator"));
            if (myLoadedAssetBundle == null)
            {
                ErrorMessage.AddError("Failed to load AssetBundle!");
                return;
            }
            System.Object[] arr = myLoadedAssetBundle.LoadAllAssets();
            foreach (System.Object obj in arr)
            {
                if (obj.ToString().Contains("attitudeindicator"))
                {
                    prefab = (GameObject)obj;
                }
            }
        }
        internal static void SetupAttitudeIndicator(Transform parent)
        {
            UnityEngine.GameObject instrumentParent = new UnityEngine.GameObject("AttitudeIndicator");
            instrumentParent.transform.SetParent(parent);
            instrumentParent.SetActive(false);
            instrumentParent.EnsureComponent<AttitudeIndicator>();
            var instrument = UnityEngine.GameObject.Instantiate(AssetGetter.prefab);
            instrument.name = "InstrumentModel";
            instrument.transform.SetParent(instrumentParent.transform);
            instrument.transform.localEulerAngles = instrument.transform.localPosition = UnityEngine.Vector3.zero;
            instrumentParent.SetActive(true);
        }
    }
}
