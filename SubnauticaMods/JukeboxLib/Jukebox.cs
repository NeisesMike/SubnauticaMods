using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JukeboxLib
{
    public abstract class Jukebox : HandTarget, IHandTarget
    {
        public enum Playback
        {
            RepeatSong,
            RepeatPlaylist,
            Shuffle
        }
        private bool isLoaded = true;
        private string currentSong = "[no song]";
        private Playback playbackStyle = Playback.RepeatPlaylist;
        public Dictionary<string, AudioClip> Playlist = new Dictionary<string, AudioClip>();

        private float _volume = 1;
        protected float Volume
        {
            set
            {
                _volume = value;
                GetSpeakers().ForEach(x => x.volume = _volume);
            }
            get
            {
                return _volume;
            }
        }

        public abstract string GetFullPathToMusicFolder();
        public abstract List<AudioSource> LeftSpeakers { get; }
        public abstract List<AudioSource> RightSpeakers { get; }
        public virtual void OnHandHover(GUIHand hand)
        {
            // show info
            string info = $"Now Playing: {currentSong}\nVolume: {Volume}";
            HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, info);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);

            // handle controls
            if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
            {
                VolumeDown();
            }
            if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
            {
                VolumeUp();
            }
            if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
            {
                if (GetSpeakers().First().isPlaying)
                {
                    Next();
                }
                else
                {
                    PlayRandom();
                }
            }
            if (GameInput.GetButtonDown(GameInput.Button.RightHand))
            {
                Stop();
            }
            return;
        }
        public virtual void OnHandClick(GUIHand hand)
        {

        }
        public virtual void FixedUpdate()
        {
            AudioSource representative = GetSpeakers().First();
            if (!representative.isPlaying) // it's not playing
            {
                if (representative.clip != null && representative.clip.length > 0) // it has a real clip
                {
                    if (representative.time > representative.clip.length) // it finished a song
                    {
                        HandlePlayNextSong();
                    }
                }
            }
        }

        #region private_methods
        private void HandlePlayNextSong()
        {
            switch (playbackStyle)
            {
                case Playback.RepeatPlaylist:
                    Next();
                    break;
                case Playback.RepeatSong:
                    Play(currentSong);
                    break;
                case Playback.Shuffle:
                    PlayRandom();
                    break;
                default:
                    throw new FormatException("Impossible Playback Style. Dying!");
            }
        }
        private List<AudioSource> GetSpeakers()
        {
            return RightSpeakers.Concat(LeftSpeakers).ToList();
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
        public void VolumeUp(float increment = 0.01f)
        {
            Volume += increment;
        }
        public void VolumeDown(float increment = 0.01f)
        {
            Volume -= increment;
        }
        public void SetVolume(float level)
        {
            Volume = level;
        }
        public void Mute(bool isMuted)
        {
            foreach (AudioSource source in GetSpeakers())
            {
                source.mute = isMuted;
            }
        }
        public void SetPlayback(Playback input)
        {
            playbackStyle = input;
        }
        #endregion
    }
}
