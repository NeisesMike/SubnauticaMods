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
        }
        public static TechType RegisterJukebox()
        {
            Nautilus.Assets.PrefabInfo Info = Nautilus.Assets.PrefabInfo.WithTechType("SimpleJukebox", "Simple Jukebox", "It can play your music.")
                .WithIcon(SpriteManager.Get(TechType.Nickel));
            Nautilus.Assets.CustomPrefab prefab = new Nautilus.Assets.CustomPrefab(Info);
            Nautilus.Utility.ConstructableFlags constructableFlags = Nautilus.Utility.ConstructableFlags.Inside | Nautilus.Utility.ConstructableFlags.Wall | Nautilus.Utility.ConstructableFlags.Submarine;
            Nautilus.Assets.PrefabTemplates.CloneTemplate lockerClone = new Nautilus.Assets.PrefabTemplates.CloneTemplate(Info, "cd34fecd-794c-4a0c-8012-dd81b77f2840");
            lockerClone.ModifyPrefab += obj =>
            {
                obj.AddComponent<DefaultJukebox>();
                GameObject model = obj.transform.Find("submarine_locker_04").gameObject;
                Nautilus.Utility.PrefabUtils.AddConstructable(obj, Info.TechType, constructableFlags, model);
            };
            //Nautilus.Utility.PrefabUtils.AddBasicComponents(cube, "SimpleJukebox", Info.TechType, LargeWorldEntity.CellLevel.Medium);
            //Nautilus.Utility.PrefabUtils.AddConstructable(cube, Info.TechType, constructableFlags, cube.transform.Find("cube2").gameObject);
            prefab.SetGameObject(lockerClone);
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            prefab.SetRecipe(new Nautilus.Crafting.RecipeData(new CraftData.Ingredient(TechType.ComputerChip, 1), new CraftData.Ingredient(TechType.Glass, 1), new CraftData.Ingredient(TechType.Titanium, 1), new CraftData.Ingredient(TechType.Silver, 1)));
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
