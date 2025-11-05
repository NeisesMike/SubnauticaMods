using System.IO;
using System.Reflection;
using UnityEngine;

namespace AttitudeIndicator
{
    internal static class AssetGetter
    {
        private static GameObject _prefab = null;
        internal static GameObject Prefab
        {
            get
            {
                if(_prefab == null)
                {
                    GetAssets();
                }
                return _prefab;
            }
        }
        internal static void GetAssets()
        {
            // load the asset bundle
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AssetBundle myLoadedAssetBundle;
            try
            {
                myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "assets/attitudeindicator"));
            }
            catch(System.Exception e)
            {
                ErrorMessage.AddError("Failed to load Attitude Indicator asset bundle!");
                throw new System.Exception("Failed to load Attitude Indicator asset bundle!\n" + e.Message);
            }
            _prefab = myLoadedAssetBundle.LoadAsset<GameObject>("attitudeindicator.prefab");
            myLoadedAssetBundle.Unload(false);
            if (_prefab == null)
            {
                ErrorMessage.AddError("Failed to load Attitude Indicator prefab!");
                throw new System.Exception("Failed to load Attitude Indicator prefab!");
            }
        }
        internal static void SetupAttitudeIndicator(Transform parent)
        {
            UnityEngine.GameObject instrumentParent = new UnityEngine.GameObject("AttitudeIndicator");
            instrumentParent.transform.SetParent(parent);
            instrumentParent.SetActive(false);
            instrumentParent.EnsureComponent<AttitudeIndicator>();
            var instrument = UnityEngine.GameObject.Instantiate(AssetGetter.Prefab);
            if(instrument == null)
            {
                ErrorMessage.AddError("Failed to instantiate Attitude Indicator prefab!");
                throw new System.Exception("Failed to instantiate Attitude Indicator prefab!");
            }
            instrument.name = "InstrumentModel";
            instrument.transform.SetParent(instrumentParent.transform);
            instrument.transform.localEulerAngles = instrument.transform.localPosition = UnityEngine.Vector3.zero;
            instrumentParent.SetActive(true);
        }
    }
}
