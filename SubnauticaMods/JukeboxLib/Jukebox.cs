using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JukeboxLib
{
    public abstract class Jukebox : HandTarget
    {
        public enum RepeatStyle
        {
            None,
            Song,
            Playlist
        }
        private string currentSong = "[no song]";
        protected string CurrentSong
        {
            get
            {
                return currentSong;
            }
        }
        protected RepeatStyle repeatStyle = RepeatStyle.None;
        protected bool isShuffle = false;

        public Dictionary<string, AudioClip> Playlist = new Dictionary<string, AudioClip>();
        public float MaxAudibleDistance = 30f;
        private int _masterVolume = 0;
        protected int MasterVolume
        {
            set
            {
                _masterVolume = value;
                _masterVolume = Math.Max(0, Math.Min(100, _masterVolume));
            }
            get
            {
                return _masterVolume;
            }
        }
        public abstract string GetFullPathToMusicFolder();
        public abstract List<AudioSource> LeftSpeakers { get; }
        public abstract List<AudioSource> RightSpeakers { get; }
        public virtual void Update()
        {
            UpdateSongFinished();
            UpdateLowPassFilter();
            UpdateVolume();
        }
        public virtual void Start()
        {
            MasterVolume = 0;
            LeftSpeakers.Concat(RightSpeakers).ForEach(SetupSpeaker);
        }
        protected virtual void OnRepeatStyleChanged(RepeatStyle repeatStyle) { }
        protected virtual void OnShuffleChanged(bool shuffling) { }
        protected virtual void OnPlaySong(string songName) { }
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
        private void SetupSpeaker(AudioSource speaker)
        {
            speaker.gameObject.EnsureComponent<AudioLowPassFilter>().cutoffFrequency = 1500;
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
            LeftSpeakers.Concat(RightSpeakers).ForEach(x => x.GetComponent<AudioLowPassFilter>().enabled = shouldEnableLowPass);
        }
        private void UpdateSongFinished()
        {
            AudioSource representative = GetSpeakers().First();
            if (!representative.isPlaying) // it's not playing
            {
                if (representative.clip != null && representative.clip.length > 0) // it has a real clip
                {
                    if (representative.time > representative.clip.length) // it finished a song
                    {
                        OnSongEnd();
                    }
                }
            }
        }
        private void OnSongEnd()
        {
            switch (repeatStyle)
            {
                case RepeatStyle.Song:
                    Play(currentSong);
                    break;
                case RepeatStyle.Playlist:
                    TryPlayNextSong(true);
                    break;
                case RepeatStyle.None:
                    TryPlayNextSong(false);
                    break;
                default:
                    throw new FormatException("Impossible Playback Style. Dying!");
            }
                    PlayRandom();
        }
        private void TryPlayNextSong(bool repeat)
        {
            if (isShuffle)
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
            return RightSpeakers.Concat(LeftSpeakers).ToList();
        }
        protected string GetSongNameFromFullPath(string fullpath)
        {
            return fullpath.SplitByChar('\\').Last();
        }
        #endregion

        #region public_methods
        public void Play(string filename)
        {
            if(Playlist.Keys.Contains(filename))
            {
                foreach(AudioSource source in GetSpeakers())
                {
                    source.clip = Playlist[filename];
                    source.Play();
                }
                currentSong = filename;
                OnPlaySong(GetSongNameFromFullPath(currentSong));
            }
            else
            {
                Logger.Log("Playlist did not contain the song: " + filename);
            }
        }
        public void Stop()
        {
            foreach (AudioSource source in GetSpeakers())
            {
                source.Stop();
            }
        }
        public void Pause(bool isPaused)
        {
            if (isPaused)
            {
                foreach (AudioSource source in GetSpeakers())
                {
                    source.Pause();
                }
            }
            else
            {
                foreach (AudioSource source in GetSpeakers())
                {
                    source.UnPause();
                }
            }
        }
        public void Next()
        {
            bool isNext = false;
            foreach(string name in Playlist.Keys)
            {
                if(isNext)
                {
                    Play(name);
                    return;
                }
                if(currentSong.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    isNext = true;
                }
            }
            Play(Playlist.Keys.First());
        }
        public void Previous()
        {
            string lastSong = Playlist.Keys.Last();
            foreach(string name in Playlist.Keys)
            {
                if (currentSong.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    Play(lastSong);
                    return;
                }
                lastSong = name;
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
        public void IncrementRepeatStyle()
        {
            switch (repeatStyle)
            {
                case RepeatStyle.None:
                    repeatStyle = RepeatStyle.Song;
                    break;
                case RepeatStyle.Song:
                    repeatStyle = RepeatStyle.Playlist;
                    break;
                case RepeatStyle.Playlist:
                    repeatStyle = RepeatStyle.None;
                    break;
            }
            OnRepeatStyleChanged(repeatStyle);
        }
        public void ToggleShuffleStyle()
        {
            isShuffle = !isShuffle;
            OnShuffleChanged(isShuffle);
        }
        #endregion
    }
}
