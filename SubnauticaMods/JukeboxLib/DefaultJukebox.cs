using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using Nautilus.Assets.Gadgets;

namespace JukeboxLib
{
    public class DefaultJukebox : Jukebox
    {
        public static Dictionary<string, AudioClip> MasterPlaylist = new Dictionary<string, AudioClip>();

        private AudioSource right;
        private AudioSource left;
        public override List<AudioSource> LeftSpeakers => new List<AudioSource>() { left };
        public override List<AudioSource> RightSpeakers => new List<AudioSource>() { right };
        public override string GetFullPathToMusicFolder()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string fullPath = Path.Combine(modPath, "music");
            return fullPath;
        }
        public override void Awake()
        {
            base.Awake();
            if (!Directory.Exists(GetFullPathToMusicFolder()))
            {
                Directory.CreateDirectory(GetFullPathToMusicFolder());
            }
            right = gameObject.AddComponent<AudioSource>();
            left = gameObject.AddComponent<AudioSource>();
            Playlist = MasterPlaylist;
            //gameObject.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => x.materials.ForEach(y => y.SetTexture("_Illum", AssetLoader.emissive)));
        }
        public static TechType RegisterJukebox()
        {
            Nautilus.Assets.PrefabInfo Info = Nautilus.Assets.PrefabInfo.WithTechType("SimpleJukebox", "Simple Jukebox", "It can play your music.")
                .WithIcon(SpriteManager.Get(TechType.Nickel));
            Nautilus.Assets.CustomPrefab prefab = new Nautilus.Assets.CustomPrefab(Info);
            Nautilus.Utility.ConstructableFlags constructableFlags = 
                Nautilus.Utility.ConstructableFlags.AllowedOnConstructable
                | Nautilus.Utility.ConstructableFlags.Default 
                | Nautilus.Utility.ConstructableFlags.Rotatable;
            /*
            Nautilus.Assets.PrefabTemplates.CloneTemplate lockerClone = new Nautilus.Assets.PrefabTemplates.CloneTemplate(Info, "cd34fecd-794c-4a0c-8012-dd81b77f2840");
            lockerClone.ModifyPrefab += obj =>
            {
                obj.AddComponent<DefaultJukebox>();
                GameObject model = obj.transform.Find("submarine_locker_04").gameObject;
                Nautilus.Utility.PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, model);
            };
            prefab.SetGameObject(lockerClone);
            */
            Nautilus.Utility.PrefabUtils.AddBasicComponents(AssetLoader.radioAsset, "SimpleJukebox", Info.TechType, LargeWorldEntity.CellLevel.Medium);
            Nautilus.Utility.PrefabUtils.AddConstructable(AssetLoader.radioAsset, Info.TechType, constructableFlags, AssetLoader.radioAsset.transform.Find("model").gameObject);
            AssetLoader.radioAsset.AddComponent<DefaultJukebox>();
            foreach(var renderer in AssetLoader.radioAsset.GetComponentsInChildren<MeshRenderer>(true))
            {
                foreach(var mat in renderer.materials)
                {
                    mat.shader = Shader.Find("MarmosetUBER");
                    mat.SetTexture("_Illum", AssetLoader.emissive);
                    mat.EnableKeyword("MARMO_EMISSION");
                    mat.EnableKeyword("MARMO_SPECMAP");
                    mat.SetFloat("_GlowStrength", 3f);
                    mat.SetFloat("_GlowStrengthNight", 3f);
                }
            }
            prefab.SetGameObject(AssetLoader.radioAsset);
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            prefab.SetRecipe(new Nautilus.Crafting.RecipeData(new CraftData.Ingredient(TechType.Titanium, 1), new CraftData.Ingredient(TechType.CopperWire, 1)));
            prefab.Register();
            return Info.TechType;
        }
        public static IEnumerator LoadMasterPlaylist(string fullPath)
        {
            var task = new TaskResult<Dictionary<string, AudioClip>>();
            yield return UWE.CoroutineHost.StartCoroutine(AudioLoader.LoadMusic(fullPath, task));
            MasterPlaylist = task.Get();
        }
    }
}
