using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using Nautilus.Assets.Gadgets;

namespace JukeboxLib
{
    public class DesktopJukebox : Jukebox, IHandTarget
    {
        public static Dictionary<string, AudioClip> MasterPlaylist = new Dictionary<string, AudioClip>();

        private AudioSource right;
        private AudioSource left;
        public override List<AudioSource> LeftSpeakers => new List<AudioSource>() { left };
        public override List<AudioSource> RightSpeakers => new List<AudioSource>() { right };
        private GameObject menuInterface => transform.Find("interface").gameObject;
        public override string GetFullPathToMusicFolder()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string fullPath = Path.Combine(modPath, "music");
            return fullPath;
        }
        public virtual void Awake()
        {
            if (!Directory.Exists(GetFullPathToMusicFolder()))
            {
                Directory.CreateDirectory(GetFullPathToMusicFolder());
            }
            right = gameObject.AddComponent<AudioSource>();
            left = gameObject.AddComponent<AudioSource>();
            Playlist = MasterPlaylist;
        }
        public static TechType RegisterJukebox()
        {
            Nautilus.Assets.PrefabInfo Info = Nautilus.Assets.PrefabInfo.WithTechType("DesktopJukebox", "Desktop Jukebox", "It can play your music.")
                .WithIcon(SpriteManager.Get(TechType.Nickel));
            Nautilus.Assets.CustomPrefab prefab = new Nautilus.Assets.CustomPrefab(Info);
            Nautilus.Utility.ConstructableFlags constructableFlags = 
                Nautilus.Utility.ConstructableFlags.AllowedOnConstructable
                | Nautilus.Utility.ConstructableFlags.Default 
                | Nautilus.Utility.ConstructableFlags.Rotatable;
            Nautilus.Utility.PrefabUtils.AddBasicComponents(AssetLoader.radioAsset, "DesktopJukebox", Info.TechType, LargeWorldEntity.CellLevel.Medium);
            Nautilus.Utility.PrefabUtils.AddConstructable(AssetLoader.radioAsset, Info.TechType, constructableFlags, AssetLoader.radioAsset.transform.Find("model").gameObject);
            AssetLoader.radioAsset.AddComponent<DesktopJukebox>();
            foreach(var renderer in AssetLoader.radioAsset.GetComponentsInChildren<MeshRenderer>(true))
            {
                if(renderer.gameObject.name.Equals("background", System.StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var mat in renderer.materials)
                    {
                        mat.shader = Shader.Find("MarmosetUBER");
                    }
                    continue;
                }
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

        private float timeToDie = 0f;
        private void ExtendTimeToDie()
        {
            timeToDie = Time.time + 1f;
        }
        public virtual void OnHandClick(GUIHand hand)
        {
            menuInterface.SetActive(true);
            ExtendTimeToDie();
        }
        public virtual void OnHandHover(GUIHand hand)
        {
            string info = $"Now Playing: {GetSongNameFromFullPath(CurrentSong)}\nVolume: {MasterVolume}%";
            HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, info);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            ExtendTimeToDie();
        }
        public override void Update()
        {
            base.Update();
            if(timeToDie < Time.time)
            {
                menuInterface.SetActive(false);
            }
            if (menuInterface.activeInHierarchy)
            {
                menuInterface.transform.LookAt(Player.main.transform);
            }
        }
    }
}
