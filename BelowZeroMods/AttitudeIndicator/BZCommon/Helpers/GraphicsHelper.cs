using System;
using System.IO;
using UnityEngine;

namespace BZCommon.Helpers
{
    public static class GraphicsHelper
    {
        public static Material GetResourceMaterial(string gameObjectPath, string meshName, int materialIndex)
        {
            GameObject gameObject = Resources.Load<GameObject>(gameObjectPath);

            foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (meshRenderer.name.Equals(meshName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Material material = UnityEngine.Object.Instantiate(meshRenderer.materials[materialIndex]) as Material;

                    UnityEngine.Object.Destroy(gameObject);

                    return material;
                }
            }

            return null;
        }


        public static void SetMeshMaterial(GameObject model, Material newMaterial, int index)
        {
            Renderer renderer = model.GetComponent<Renderer>();

            Material[] materials = renderer.materials;

            materials.SetValue(newMaterial, index);

            renderer.materials = materials;
        }

        public static void SetMeshMaterial(GameObject model, Material newMaterial, int index, string newMaterialName)
        {
            Renderer renderer = model.GetComponent<Renderer>();

            Material[] materials = renderer.materials;

            materials.SetValue(newMaterial, index);

            materials[index].name = newMaterialName;

            renderer.materials = materials;
        }

        public static void ChangeObjectTexture(GameObject model, int materialIndex, Texture2D mainTex = null, Texture2D illumTex = null, Texture2D bumpMap = null, Texture2D specTex = null)
        {
            Renderer renderer = model.GetComponent<Renderer>();

            if (materialIndex < 0 || materialIndex >= renderer.materials.Length)
            {
                throw new ArgumentException("Material index is out of range!");
            }

            if (mainTex != null)
            {
                renderer.materials[materialIndex].SetTexture(ShaderPropertyID._MainTex, mainTex);
            }

            if (illumTex != null)
            {
                renderer.materials[materialIndex].SetTexture(ShaderPropertyID._Illum, illumTex);
            }

            if (bumpMap != null)
            {
                renderer.materials[materialIndex].SetTexture(ShaderPropertyID._BumpMap, bumpMap);
            }

            if (specTex != null)
            {
                renderer.materials[materialIndex].SetTexture(ShaderPropertyID._SpecTex, specTex);
            }
        }

        public static Texture2D GetTextureFromRenderer(Renderer renderer, string shaderPropertyKeyword)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty(shaderPropertyKeyword))
                {
                    Texture2D texture = (Texture2D)material.GetTexture(Shader.PropertyToID(shaderPropertyKeyword));

                    return CreateRWTextureFromNonReadableTexture(texture);
                }
            }

            return null;
        }

        public static Texture2D CreateRWTextureFromNonReadableTexture(Texture2D texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, tmp);

            RenderTexture previous = RenderTexture.active;

            RenderTexture.active = tmp;

            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false, true);

            newTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);

            newTexture.Apply();

            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(tmp);

            return newTexture;
        }

        public static void WriteTextureToPNG(Texture2D texture, string filename)
        {
            byte[] textureData = texture.EncodeToPNG();

            string pngPath = Path.Combine(Environment.CurrentDirectory, $"{filename}.png");

            File.WriteAllBytes(pngPath, textureData);
        }
    }
}
