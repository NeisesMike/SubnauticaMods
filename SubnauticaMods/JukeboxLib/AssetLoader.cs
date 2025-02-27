using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace JukeboxLib
{
    internal static class AssetLoader
    {
        internal static GameObject radioAsset;
        internal static Texture2D emissive;
        internal static void LoadRadioAsset()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string bundlePath = Path.Combine(directoryPath, "radio");
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            System.Object[] objectArray = bundle.LoadAllAssets();
            foreach (System.Object obj in objectArray)
            {
                if (obj.ToString().Contains("Radio"))
                {
                    radioAsset = (GameObject)obj;
                }
                if (obj.ToString().Contains("Emissive"))
                {
                    emissive = (Texture2D)obj;
                }
            }
            if(radioAsset == null)
            {
                ErrorMessage.AddError("JukeboxLib: failed to get Radio Asset!");
            }
            if (emissive == null)
            {
                ErrorMessage.AddError("JukeboxLib: failed to get emissive texture!");
            }
        }
    }
}
