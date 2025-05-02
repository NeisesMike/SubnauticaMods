using UnityEngine;
using System.IO;
using System.Reflection;

namespace JukeboxLib
{
    internal static class AssetLoader
    {
        internal static GameObject radioAsset;
        internal static Texture2D emissive;
        internal static Sprite crafterSprite;
        internal static void LoadRadioAsset()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string bundlePath = Path.Combine(directoryPath, "radio");
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            radioAsset = bundle.LoadAsset<GameObject>("Radio.prefab");
            emissive = bundle.LoadAsset<Texture2D>("radio_new_EmissiveNew.png");
            UnityEngine.U2D.SpriteAtlas spriteAtlas = bundle.LoadAsset<UnityEngine.U2D.SpriteAtlas>("JukeboxSpriteAtlas.spriteatlas");
            crafterSprite = spriteAtlas.GetSprite("JukeboxCrafterSprite");

            if(radioAsset == null)
            {
                ErrorMessage.AddError("JukeboxLib: failed to get Radio Asset!");
            }
            if (emissive == null)
            {
                ErrorMessage.AddError("JukeboxLib: failed to get emissive texture!");
            }
            if (crafterSprite == null)
            {
                ErrorMessage.AddError("JukeboxLib: failed to get crafter sprite!");
            }
        }
    }
}
