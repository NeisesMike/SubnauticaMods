using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace JukeboxLib
{
    public static class AudioLoader
    {
        private struct MusicRequest
        {
            internal string songName;
            internal string fullPath;
            internal Coroutine cor;
            internal TaskResult<AudioClip> task;
        }

        public static List<String> ReadMusicFilenames(string directoryFullPath)
        {
            try
            {
                return Directory.GetFiles(directoryFullPath).ToList();
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.Log("The directory does not exist.");
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Log("You do not have permission to access this directory.");
                throw e;
            }
            catch (Exception ex)
            {
                Logger.Log($"An error occurred: {ex.Message}");
                throw ex;
            }
        }

        public static bool CheckMusicDirty(string directoryFullPath, List<string> lastKnown)
        {
            List<string> current = ReadMusicFilenames(directoryFullPath);
            return !current.SequenceEqual(lastKnown);
        }

        public static IEnumerator LoadMusic(string directoryFullPath, IOut<Dictionary<string, AudioClip>> result)
        {
            List<MusicRequest> requests = new List<MusicRequest>();
            foreach(string file in Directory.GetFiles(directoryFullPath))
            {
                MusicRequest thisRequest;
                thisRequest.songName = file;
                thisRequest.fullPath = Path.Combine(directoryFullPath, file);
                thisRequest.task = new TaskResult<AudioClip>();
                thisRequest.cor = UWE.CoroutineHost.StartCoroutine(LoadAudio(thisRequest.fullPath, thisRequest.task));
                requests.Add(thisRequest);
            }
            Dictionary<string, AudioClip> output = new Dictionary<string, AudioClip>();
            foreach (MusicRequest req in requests)
            {
                yield return req.cor;
                output.Add(req.songName, req.task.Get());
            }
            result.Set(output);
        }

        static IEnumerator LoadAudio(string path, IOut<AudioClip> result)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.LogError($"Error loading audio: {www.error}");
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        result.Set(clip);
                    }
                    else
                    {
                        Debug.LogError("Failed to load audio clip.");
                    }
                }
            }
        }
    }

}
