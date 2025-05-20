using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JukeboxLib
{
    public enum RepeatEnum
    {
        None,
        Song,
        Playlist
    }
    public abstract class Jukebox : HandTarget
    {
        private bool isCustomMusic = true;
        private Dictionary<string, AudioClip> InternalPlaylist = new Dictionary<string, AudioClip>();
        protected Dictionary<string, AudioClip> Playlist = new Dictionary<string, AudioClip>();
        public const string noSongString = "[no song]";
        private string currentSong = noSongString;
        protected RepeatEnum RepeatStyle { get; private set; } = RepeatEnum.Playlist;
        protected bool IsShuffle { get; private set; } = false;
        protected bool IsPaused { get; private set; } = false;

        protected float MaxAudibleDistance = 30f;
        private int _masterVolume = 0;
        protected int MasterVolume
        {
            private set
            {
                _masterVolume = value;
                _masterVolume = Math.Max(0, Math.Min(100, _masterVolume));
            }
            get
            {
                return _masterVolume;
            }
        }

        #region passthrough
        protected abstract List<AudioSource> LeftSpeakers { get; }
        protected abstract List<AudioSource> RightSpeakers { get; }
        protected virtual void Update()
        {
            UpdateSongFinished();
            UpdateLowPassFilter();
            UpdateVolume();
            UpdateSongProgress();
        }
        protected virtual void Start()
        {
            MasterVolume = 0;
            OnShuffleChanged(IsShuffle);
            OnRepeatStyleChanged(RepeatStyle);
            FreezeTimePatcher.Register(this);
        }
        protected virtual void OnRepeatStyleChanged(RepeatEnum repeatStyle) { }
        protected virtual void OnShuffleChanged(bool shuffling) { }
        protected virtual void OnPlaySong(string songName) { }
        protected virtual void OnPlay() { }
        protected virtual void OnPause() { }
        protected virtual void OnSongProgress(float progress) { }
        protected virtual void OnStopped() { }
        protected virtual void OnCustomMusicSelected() { }
        protected virtual void OnIncludedMusicSelected() { }
        #endregion

        #region private_methods
        private void UpdateVolume()
        {
            float desiredVolume;
            float playerDistance = Vector3.Distance(Player.main.transform.position, transform.position);
            if (MaxAudibleDistance < playerDistance)
            {
                desiredVolume = 0;
            }
            else
            {
                desiredVolume = ((float)_masterVolume / 100f) * ((MaxAudibleDistance - playerDistance) / MaxAudibleDistance);
            }
            GetSpeakers().ForEach(x => x.volume = desiredVolume);
        }
        private void UpdateLowPassFilter()
        {
            bool shouldEnableLowPass = true;
            if (Player.main.IsInBase())
            {
                BaseRoot baseroot = gameObject.GetComponentInParent<BaseRoot>();
                if (baseroot != null && baseroot == Player.main.currentSub.GetComponent<BaseRoot>())
                {
                    // the player and jukebox are in the same base
                    shouldEnableLowPass = false;
                }
            }
            else if(Player.main.currentSub != null)
            {
                SubRoot thisSubRoot = gameObject.GetComponentInParent<SubRoot>();
                if (thisSubRoot != null && thisSubRoot == Player.main.currentSub)
                {
                    // the player and jukebox are in the same submarine
                    shouldEnableLowPass = false;
                }
            }
            GetSpeakers().ForEach(x => x.GetComponent<AudioLowPassFilter>().enabled = shouldEnableLowPass);
        }
        private void UpdateSongFinished()
        {
            AudioSource representative = GetSpeakers().First();
            if (!representative.isPlaying) // it's not playing
            {
                if (representative.clip != null && representative.clip.length > 0) // it has a real clip
                {
                    if (representative.time >= representative.clip.length) // it finished a song
                    {
                        OnSongEnd();
                    }
                }
            }
        }
        private void OnSongEnd()
        {
            switch (RepeatStyle)
            {
                case RepeatEnum.Song:
                    Play(currentSong);
                    break;
                case RepeatEnum.Playlist:
                    TryPlayNextSong(true);
                    break;
                case RepeatEnum.None:
                    TryPlayNextSong(false);
                    break;
                default:
                    throw new FormatException("Impossible Playback Style. Dying!");
            }
        }
        private void TryPlayNextSong(bool repeat)
        {
            if (IsShuffle)
            {
                PlayRandom();
                return;
            }
            if (repeat)
            {
                if (currentSong == Playlist.Last().Key)
                {
                    Play(Playlist.First().Key);
                }
                else
                {
                    Next();
                }
            }
            else
            {
                if (currentSong == Playlist.Last().Key)
                {
                    Stop();
                }
                else
                {
                    Next();
                }
            }
        }
        private List<AudioSource> GetSpeakers()
        {
            List<AudioSource> result = RightSpeakers.Concat(LeftSpeakers).ToList();
            result.ForEach(x => x.gameObject.EnsureComponent<AudioLowPassFilter>().cutoffFrequency = 1500);
            return result;
        }
        private string GetSongNameFromFullPath(string fullpath)
        {
            return fullpath.SplitByChar('\\').Last();
        }
        private void UpdateSongProgress()
        {
            if (IsPlaying())
            {
                AudioSource representative = GetSpeakers().First();
                if (representative.clip == null || representative.clip.length == 0) return;

                OnSongProgress(representative.time / representative.clip.length);
            }
        }
        #endregion

        #region internal_methods
        private bool isMenuPause = false;
        internal void MenuPause(bool shouldPause)
        {
            if (shouldPause)
            {
                if (IsPlaying())
                {
                    Pause(true);
                    isMenuPause = true;
                }
            }
            else
            {
                if (isMenuPause)
                {
                    Pause(false);
                }
                isMenuPause = false;
            }
        }
        #endregion

        #region public_methods
        public void PlayFirst()
        {
            if (IsShuffle)
            {
                PlayRandom();
            }
            else
            {
                if (Playlist.Any())
                {
                    Play(Playlist.First().Key);
                }
            }
        }
        public void Play(string filename)
        {
            if (Playlist.Any())
            {
                if (Playlist.Keys.Contains(filename))
                {
                    foreach (AudioSource source in GetSpeakers())
                    {
                        source.clip = Playlist[filename];
                        source.time = 0;
                        source.Play();
                    }
                    currentSong = filename;
                    OnPlaySong(GetSongNameFromFullPath(currentSong));
                    IsPaused = false;
                    OnPlay();
                }
                else
                {
                    Logger.Log("Playlist did not contain the song: " + filename);
                }
            }
        }
        public void PressPlayButton()
        {
            if (IsPlaying())
            {
                Pause(true);
            }
            else
            {
                if (IsPaused)
                {
                    Pause(false);
                }
                else
                {
                    PlayFirst();
                }
            }
        }
        public void Stop()
        {
            foreach (AudioSource source in GetSpeakers())
            {
                source.Stop();
            }
            IsPaused = false;
            currentSong = noSongString;
            OnStopped();
        }
        public void Pause(bool inputPaused)
        {
            if (inputPaused)
            {
                foreach (AudioSource source in GetSpeakers())
                {
                    source.Pause();
                }
                OnPause();
                IsPaused = true;
            }
            else
            {
                foreach (AudioSource source in GetSpeakers())
                {
                    source.UnPause();
                }
                OnPlay();
                IsPaused = false;
            }
        }
        public void Next()
        {
            if (Playlist.Any())
            {
                bool isNext = false;
                if (IsShuffle)
                {
                    PlayRandom();
                    return;
                }
                foreach (string name in Playlist.Keys)
                {
                    if (isNext)
                    {
                        Play(name);
                        return;
                    }
                    if (currentSong.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isNext = true;
                    }
                }
                Play(Playlist.Keys.First());
            }
        }
        public void Previous()
        {
            if (Playlist.Any())
            {
                string lastSong = Playlist.Keys.Last();
                foreach (string name in Playlist.Keys)
                {
                    if (currentSong.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        Play(lastSong);
                        return;
                    }
                    lastSong = name;
                }
            }
        }
        public void PlayRandom()
        {
            if (Playlist.Any())
            {
                var random = new System.Random();
                int index = random.Next(Playlist.Count());
                Play(Playlist.Keys.ToList()[index]);
            }
        }
        public void VolumeUp(int increment = 1)
        {
            MasterVolume += increment;
        }
        public void VolumeDown(int increment = 1)
        {
            MasterVolume -= increment;
        }
        public void SetVolume(int level)
        {
            MasterVolume = level;
        }
        public void Mute(bool isMuted)
        {
            foreach (AudioSource source in GetSpeakers())
            {
                source.mute = isMuted;
            }
        }
        public void SetRepeat(RepeatEnum style)
        {
            RepeatStyle = style;
            OnRepeatStyleChanged(RepeatStyle);
        }
        public void ToggleShuffleStyle()
        {
            IsShuffle = !IsShuffle;
            OnShuffleChanged(IsShuffle);
        }
        public bool IsPlaying()
        {
            return GetSpeakers().First().isPlaying;
        }
        public void SetSongProgress(float progress)
        {
            if(progress < 0)
            {
                throw new System.ArgumentException("Can't set song progress to a negative time value.", "progress");
            }
            if(progress > 1)
            {
                throw new System.ArgumentException("Can't set song progress to a time value larger than the length of the song.", "progress");
            }
            GetSpeakers().ForEach(x => x.time = x.clip.length * progress);
        }
        public void ToggleCustomMusic()
        {
            isCustomMusic = !isCustomMusic;
            if (isCustomMusic)
            {
                Playlist = InternalPlaylist;
                OnCustomMusicSelected();
            }
            else
            {
                Playlist = JukeboxLibrary.GetIncludedMusic();
                OnIncludedMusicSelected();
            }
        }
        public void SetPlaylist(Dictionary<string, AudioClip> inputPlaylist)
        {
            InternalPlaylist = Playlist = new Dictionary<string, AudioClip>(inputPlaylist);
            if (!isCustomMusic)
            {
                ToggleCustomMusic();
            }
        }
        public void AddSpeakerLeft(AudioSource source)
        {
            LeftSpeakers.Add(source);
            GetSpeakers();
        }
        public void AddSpeakerRight(AudioSource source)
        {
            RightSpeakers.Add(source);
            GetSpeakers();
        }
        #endregion

        #region static_methods
        public static string GetFullPathToMusicFolder()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(modPath, "music");
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            return fullPath;
        }
        #endregion
    }
}
