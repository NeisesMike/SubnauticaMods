using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using JukeboxLib;

namespace SimpleJukebox
{
    public class DesktopJukebox : Jukebox, IHandTarget
    {
        private string _currentSongName = noSongString;
        public string CurrentSongName
        {
            get
            {
                return _currentSongName;
            }
            private set
            {
                transform.Find("interface/background/Canvas/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = _currentSongName = value;
            }
        }
        private GameObject PlayImage => transform.Find("interface/background/play/Canvas/play").gameObject;
        private GameObject PauseImage => transform.Find("interface/background/play/Canvas/pause").gameObject;
        private GameObject LoopImage => transform.Find("interface/background/repeat/Canvas/loop").gameObject;
        private GameObject LoopSingleImage => transform.Find("interface/background/repeat/Canvas/loopsingle").gameObject;
        private GameObject NoLoopImage => transform.Find("interface/background/repeat/Canvas/noloop").gameObject;
        private GameObject ShuffleImage => transform.Find("interface/background/playback/Canvas/shuffle").gameObject;
        private GameObject NoShuffleImage => transform.Find("interface/background/playback/Canvas/noshuffle").gameObject;
        private GameObject CustomMusicImage => transform.Find("interface/background/custom/Canvas/custommusic").gameObject;
        private GameObject IncludedMusicImage => transform.Find("interface/background/custom/Canvas/includedmusic").gameObject;
        public static Dictionary<string, AudioClip> MasterPlaylist = new Dictionary<string, AudioClip>();

        private AudioSource right;
        private AudioSource left;
        protected override List<AudioSource> LeftSpeakers => new List<AudioSource>() { left };
        protected override List<AudioSource> RightSpeakers => new List<AudioSource>() { right };
        private GameObject MenuInterface => transform.Find("interface").gameObject;
        private Transform Tracker => transform.Find("interface/background/time/tracker");
        public override void Awake()
        {
            base.Awake();
            right = gameObject.AddComponent<AudioSource>();
            left = gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Stoneroom;
            SetPlaylist(MasterPlaylist);
            SetupButtons();
            CurrentSongName = noSongString;
        }
        public static TechType RegisterJukebox()
        {
            Nautilus.Assets.PrefabInfo Info = Nautilus.Assets.PrefabInfo.WithTechType("DesktopJukebox", "Desktop Jukebox", "It can play your music.", unlockAtStart: true)
                .WithIcon(new Atlas.Sprite(AssetLoader.crafterSprite));
            Nautilus.Assets.CustomPrefab prefab = new Nautilus.Assets.CustomPrefab(Info);
            Nautilus.Utility.ConstructableFlags constructableFlags =
                Nautilus.Utility.ConstructableFlags.AllowedOnConstructable
                | Nautilus.Utility.ConstructableFlags.Default
                | Nautilus.Utility.ConstructableFlags.Rotatable;
            Nautilus.Utility.PrefabUtils.AddBasicComponents(AssetLoader.radioAsset, "DesktopJukebox", Info.TechType, LargeWorldEntity.CellLevel.Medium);
            Nautilus.Utility.PrefabUtils.AddConstructable(AssetLoader.radioAsset, Info.TechType, constructableFlags, AssetLoader.radioAsset.transform.Find("model").gameObject);
            AssetLoader.radioAsset.AddComponent<DesktopJukebox>();
            foreach (var renderer in AssetLoader.radioAsset.transform.Find("model").GetComponentsInChildren<MeshRenderer>(true))
            {
                if (renderer.gameObject.name.Equals("background", System.StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var mat in renderer.materials)
                    {
                        mat.shader = Shader.Find("MarmosetUBER");
                    }
                    continue;
                }
                foreach (var mat in renderer.materials)
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
        public static IEnumerator LoadMasterPlaylist()
        {
            var task = new TaskResult<Dictionary<string, AudioClip>>();
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(modPath, "music");
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
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
            MenuInterface.SetActive(true);
            ExtendTimeToDie();
        }
        public virtual void OnHandHover(GUIHand hand)
        {
            string info = $"Now Playing: {CurrentSongName}\nVolume: {MasterVolume}%\n";
            info += HandReticle.main.GetText("Volume Up:   ", false, GameInput.Button.CycleNext) + "\n";
            info += HandReticle.main.GetText("Volume Down: ", false, GameInput.Button.CyclePrev) + "\n";
            info += HandReticle.main.GetText("Stop: ", false, GameInput.Button.RightHand) + "\n";
            info += HandReticle.main.GetText("Open Menu: ", false, GameInput.Button.LeftHand) + "\n";
            HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, info);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            ExtendTimeToDie();
            if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
            {
                VolumeDown();
            }
            if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
            {
                VolumeUp();
            }
            if (GameInput.GetButtonDown(GameInput.Button.RightHand))
            {
                Stop();
            }
        }
        protected override void Update()
        {
            base.Update();
            if (timeToDie < Time.time)
            {
                MenuInterface.SetActive(false);
            }
            if (MenuInterface.activeInHierarchy)
            {
                MenuInterface.transform.LookAt(Player.main.transform);
            }
            GetComponent<AudioReverbFilter>().reverbPreset = MainPatcher.DesktopJukeboxConfig.ReverbPreset.Value;
        }

        protected override void OnShuffleChanged(bool shuffling)
        {
            if (shuffling)
            {
                ShuffleImage.SetActive(true);
                NoShuffleImage.SetActive(false);
            }
            else
            {
                ShuffleImage.SetActive(false);
                NoShuffleImage.SetActive(true);
            }
        }
        protected override void OnRepeatStyleChanged(RepeatEnum repeatStyle)
        {
            switch (repeatStyle)
            {
                case RepeatEnum.None:
                    LoopImage.SetActive(false);
                    LoopSingleImage.SetActive(false);
                    NoLoopImage.SetActive(true);
                    break;
                case RepeatEnum.Song:
                    LoopImage.SetActive(false);
                    LoopSingleImage.SetActive(true);
                    NoLoopImage.SetActive(false);
                    break;
                case RepeatEnum.Playlist:
                    LoopImage.SetActive(true);
                    LoopSingleImage.SetActive(false);
                    NoLoopImage.SetActive(false);
                    break;
            }
        }
        protected override void OnPlaySong(string songName)
        {
            CurrentSongName = songName;
        }
        protected override void OnPlay()
        {
            PlayImage.SetActive(false);
            PauseImage.SetActive(true);
        }
        protected override void OnPause()
        {
            PlayImage.SetActive(true);
            PauseImage.SetActive(false);
        }
        protected override void OnStopped()
        {
            CurrentSongName = noSongString;
            PlayImage.SetActive(true);
            PauseImage.SetActive(false);
        }
        protected override void OnSongProgress(float progress)
        {
            float placement = Mathf.Lerp(-0.5f, 0.5f, progress);
            Tracker.localPosition = new Vector3(0f, placement, 0f);
        }
        protected override void OnCustomMusicSelected()
        {
            CustomMusicImage.SetActive(true);
            IncludedMusicImage.SetActive(false);
        }
        protected override void OnIncludedMusicSelected()
        {
            CustomMusicImage.SetActive(false);
            IncludedMusicImage.SetActive(true);
        }
        private void SetupButtons()
        {
            void HoverAction(string message)
            {
                ExtendTimeToDie();
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, message);
            }
            JukeboxButton play = transform.Find("interface/background/play").gameObject.AddComponent<JukeboxButton>();
            play.clickAction = PressPlayButton;
            play.hoverAction = () => HoverAction("Play");
            play.GetComponent<MeshRenderer>().sortingOrder = 1;

            JukeboxButton prev = transform.Find("interface/background/previous").gameObject.AddComponent<JukeboxButton>();
            prev.clickAction = Previous;
            prev.hoverAction = () => HoverAction("Previous");
            prev.GetComponent<MeshRenderer>().sortingOrder = 1;

            JukeboxButton next = transform.Find("interface/background/next").gameObject.AddComponent<JukeboxButton>();
            next.clickAction = Next;
            next.hoverAction = () => HoverAction("Next");
            next.GetComponent<MeshRenderer>().sortingOrder = 1;

            JukeboxButton shuffle = transform.Find("interface/background/playback").gameObject.AddComponent<JukeboxButton>();
            shuffle.clickAction = ToggleShuffleStyle;
            shuffle.hoverAction = () => HoverAction("Shuffle");
            shuffle.GetComponent<MeshRenderer>().sortingOrder = 1;

            JukeboxButton repeat = transform.Find("interface/background/repeat").gameObject.AddComponent<JukeboxButton>();
            repeat.clickAction = IncrementRepeatStyle;
            repeat.hoverAction = () => HoverAction("Repeat");
            repeat.GetComponent<MeshRenderer>().sortingOrder = 1;

            Tracker.GetComponent<MeshRenderer>().sortingOrder = 2;
            Tracker.parent.GetComponent<MeshRenderer>().sortingOrder = 1;

            JukeboxButton seek = Tracker.parent.gameObject.AddComponent<JukeboxButton>();
            seek.clickAction = SeekSong;
            seek.hoverAction = () => HoverAction("Seek");

            JukeboxButton custom = transform.Find("interface/background/custom").gameObject.AddComponent<JukeboxButton>();
            custom.clickAction = ToggleCustomMusic;
            custom.hoverAction = () => HoverAction("Custom Music");
            custom.GetComponent<MeshRenderer>().sortingOrder = 1;
        }
        public void IncrementRepeatStyle()
        {
            switch (RepeatStyle)
            {
                case RepeatEnum.None:
                    SetRepeat(RepeatEnum.Song);
                    break;
                case RepeatEnum.Song:
                    SetRepeat(RepeatEnum.Playlist);
                    break;
                case RepeatEnum.Playlist:
                    SetRepeat(RepeatEnum.None);
                    break;
            }
        }
        private void SeekSong()
        {
            if (CurrentSongName == noSongString)
            {
                return;
            }
            // Get the center of the screen (in pixels)
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            // Create a ray from the camera going through the center of the screen
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == Tracker.parent.GetComponent<Collider>())
                {
                    Vector3 hitWorldPos = hit.point;

                    // Convert the world-space hit point to local-space relative to the hit object
                    Vector3 hitLocalPos = hit.collider.transform.InverseTransformPoint(hitWorldPos);

                    // the progress bar has length 1, so hitLocalPos is in [-0.5, 0.5]
                    float progress = hitLocalPos.y + 0.5f;
                    SetSongProgress(progress);
                    return;
                }
            }
        }
    }
}
