using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BZCommon.Helpers
{
    public static class ColorizationHelper
    {        
        public static void AddRendererToColorCustomizer(GameObject containCustomizer, GameObject containRenderer, bool clearCustomizer, params int[] materialIndexes)
        {
            ColorCustomizer colorCustomizer = containCustomizer.GetComponent<ColorCustomizer>();
            
            if (colorCustomizer == null)
            {
                throw new ArgumentException("GameObject does not have ColorCustomizer component!");                
            }

            Renderer renderer = containRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new ArgumentException($"GameObject [{containRenderer.name}] does not have Renderer component!");
            }

            List<ColorCustomizer.ColorData> colorDatas = colorCustomizer.colorDatas.ToList();

            if (clearCustomizer)
            {
                colorDatas.Clear();
            }
            else
            {
                foreach (ColorCustomizer.ColorData colorData in colorDatas)
                {
                    if (colorData.renderer.Equals(renderer))
                    {
                        BZLogger.Log($"ColorData.renderers [{renderer.name}] has contain the desired renderer!");
                        return;
                    }
                }
            }

            foreach (int index in materialIndexes)
            {
                colorDatas.Add(new ColorCustomizer.ColorData { renderer = renderer, materialIndex = index });
            }

            colorCustomizer.colorDatas = colorDatas.ToArray();        
        }

        
        public static void AddRendererToSkyApplier(GameObject containAppliers, GameObject containRenderer, Skies applierType)
        {
            List<SkyApplier> skyAppliers = new List<SkyApplier>();

            containAppliers.GetComponents(skyAppliers);

            if (skyAppliers.Count < 1)
            {
                throw new ArgumentException($"GameObject [{containAppliers.name}] does not have SkyApplier components!");
            }

            Renderer renderer = containRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new ArgumentException($"GameObject [{containRenderer.name}] does not have Renderer component!");
            }

            foreach (SkyApplier skyApplier in skyAppliers)
            {
                if (skyApplier.anchorSky == applierType)
                {
                    List<Renderer> renderers = skyApplier.renderers.ToList();

                    if (renderers.Contains(renderer))
                    {
                        BZLogger.Log($"SkyApplier [{renderer.name}] has contain the desired renderer!");
                        return;
                    }

                    renderers.Add(renderer);

                    skyApplier.renderers = renderers.ToArray();

                    return;
                }
            }

            throw new Exception("The desired Skies type not found in these SkyAppliers!");            
        }

        public static void RegisterRendererInLightingController(GameObject containController, GameObject containRenderer)
        {
            LightingController lightingController = containController.GetComponent<LightingController>();

            if (lightingController == null)
            {
                throw new ArgumentException("GameObject does not have LightingController component!");
            }

            Renderer renderer = containRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new ArgumentException($"GameObject [{containRenderer.name}] does not have Renderer component!");
            }

            lightingController.emissiveController.RegisterRenderer(renderer);
        }

        public static void UnregisterRendererInLightingController(GameObject containController, GameObject containRenderer)
        {
            LightingController lightingController = containController.GetComponent<LightingController>();

            if (lightingController == null)
            {
                throw new ArgumentException("GameObject does not have LightingController component!");
            }

            Renderer renderer = containRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new ArgumentException($"GameObject [{containRenderer.name}] does not have Renderer component!");
            }

            lightingController.emissiveController.UnregisterRenderer(renderer);
        }
    }
}
