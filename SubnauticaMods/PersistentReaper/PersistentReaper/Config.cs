using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Nautilus.Options.Attributes;
using Nautilus.Options;
using Nautilus.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace PersistentReaper
{
    public enum ReaperBehaviors
    {
        Normal,
        Bloodthirsty,
        HumanHunter
    }

    public enum DepthMap
    {
        Normal,
        NoShallowReapers
    }

    [Menu("Persistent Reaper Options")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Toggle Persistent Reapers"), OnChange(nameof(KillAllReapers))]
        public bool areReapersActive = true;
        public void KillAllReapers()
        {
            while(ReaperManager.reaperDict.Count < 0)
            {
                ReaperManager.RemoveOneReaper();
            }
        }

        [Slider("1000s of Persistent Reapers", Min = 0, Max = 9, Step = 1)]
        public int numThousandReapers = 0;

        [Slider("100s of Persistent Reapers", Min = 0, Max = 9, Step = 1)]
        public int numHundredReapers = 0;

        [Slider("10s of Persistent Reapers", Min = 0, Max = 9, Step = 1)]
        public int numTenReapers = 0;

        [Slider("1s of Persistent Reapers", Min = 0, Max = 9, Step = 1)]
        public int numSingleReapers = 1;


        [Choice("Persistent Reaper Behavior")]
        public ReaperBehaviors reaperBehaviors = ReaperBehaviors.Normal;

        public const string scentLifetimeTooltip = "Change how long your scent sticks around. Higher numbers make you easier to track. Consider choosing a lower number if performance is suffering.";
        [Slider("Scent Lifetime (Seconds)", Min = 0, Max = 600, Step = 5, DefaultValue = 120, Tooltip = scentLifetimeTooltip)]
        public int scentLifetime = 120;

        [Choice("Depth Map"), OnChange(nameof(SetDepthMap))]
        public DepthMap depthMapChoice = DepthMap.NoShallowReapers;

        public void SetDepthMap(ChoiceChangedEventArgs<DepthMap> e)
        {
            ReaperManager.depthDictionary = ReaperManager.GetDepthDictionary(e.Value);
            KillAllReapers();
        }

        public const string updateIntervalTooltip = "Change this to make the reapers simulate less frequently. Simulation is balanced for 1 second here, so only increase this number if performance is suffering.";
        [Slider("Reaper Update Interval (seconds)", Min = 1f, Max = 10f, Step = 0.25f, DefaultValue = 1f, Tooltip = updateIntervalTooltip)]
        public float updateInterval = 1f;

        [Button("Print Reaper Map", Tooltip = "Creates a file in the Persistent Reaper mod folder that shows where reapers are in the game world."), OnGameObjectCreated(nameof(PrintReaperMap))]
        public void PrintReaperMap(ButtonClickedEventArgs e)
        {
            int mapWidth = 256;
            int mapHeight = 256;
            using (Bitmap bitmap = new Bitmap(mapWidth, mapHeight))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // Fill the background with a default color
                    g.Clear(System.Drawing.Color.Black);
                    // Draw reaper locations
                    foreach (KeyValuePair<ReaperBehavior, GameObject> entry in ReaperManager.reaperDict)
                    {
                        Int3 location = entry.Key.currentRegion;
                        // Ensure the location is within bounds
                        if (location.x >= 0 && location.x < mapWidth && location.z >= 0 && location.z < mapHeight)
                        {
                            // Set the pixel color at the reaper's location (e.g., red for reapers)
                            bitmap.SetPixel(location.z, location.x, System.Drawing.Color.White);
                        }
                    }
                }
                // Save the bitmap as a PNG file
                string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string filePath = Path.Combine(modPath, "ReaperMap.png");
                bitmap.Save(filePath, ImageFormat.Png);
            }
        }
    }
}
