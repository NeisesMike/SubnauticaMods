using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HarmonyLib;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using LitJson;
using System.Runtime.CompilerServices;
using System.Collections;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Patchers;
using System.Threading;
using System.Collections.Concurrent;

namespace AttitudeIndicator
{
    public enum VehicleType
    {
        Seamoth,
        Cyclops,
        Seatruck,
        Snowfox,
        None
    }

    public static class AttitudeIndicator
    {
        internal static Sprite AttitudeIndicatorFrameSprite;
        internal static Sprite AttitudeIndicatorScreenSprite;
        internal static Sprite AttitudeIndicatorLadderSprite;

        internal static GameObject AttitudeIndicatorFrame;
        internal static GameObject AttitudeIndicatorScreen;
        internal static GameObject AttitudeIndicatorLadder;

        internal static SpriteRenderer ladderSpriteRenderer;
        internal static Texture2D originalLadderTexture;
        internal static Texture2D expandedLadderTexture;


        internal static int AttitudeIndicatorDiameter;

        internal static int graduation = 180;
        internal static Sprite[] ladderSpriteArray = new Sprite[graduation + 1];

        internal static int generationStep = 0;

        public static void initAttitudeIndicatorSprites(IndicatorStyle chosenStyle)
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            byte[] frameBytes;
            byte[] screenBytes;
            byte[] ladderBytes;
            switch (chosenStyle)
            {
                case IndicatorStyle.Standard:
                    frameBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Frame_Standard.png"));
                    screenBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Screen_Standard.png"));
                    ladderBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Ladder_Standard.png"));
                    break;
                case IndicatorStyle.Blue:
                    frameBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Frame_Blue.png"));
                    screenBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Screen_Blue.png"));
                    ladderBytes = System.IO.File.ReadAllBytes(Path.Combine(modPath, "assets/Alterra_UI_Ladder_Blue.png"));
                    break;
                default:
                    Logger.Log("Invalid Indicator Style chosen. Aborting.");
                    return;
            }


            Texture2D frameTexture = new Texture2D(128, 128);
            Texture2D screenTexture = new Texture2D(128, 128);

            frameTexture.LoadImage(frameBytes);
            screenTexture.LoadImage(screenBytes);

            AttitudeIndicatorFrameSprite = Sprite.Create(frameTexture, new Rect(0.0f, 0.0f, frameTexture.width, frameTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            AttitudeIndicatorScreenSprite = Sprite.Create(screenTexture, new Rect(0.0f, 0.0f, screenTexture.width, screenTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            AttitudeIndicatorDiameter = screenTexture.width;

            originalLadderTexture = new Texture2D(128, 128);
            originalLadderTexture.LoadImage(ladderBytes);

            // expand the top and bottom of the ladder, by an amount of lines = width/2;
            expandedLadderTexture = new Texture2D(originalLadderTexture.width, originalLadderTexture.height + 5 * originalLadderTexture.width / 4);

            for (int i = 0; i < originalLadderTexture.width / 2; i++)
            {
                for (int j = 0; j < originalLadderTexture.width; j++)
                {
                    expandedLadderTexture.SetPixel(j, i, Color.clear);
                }
            }
            for (int i = originalLadderTexture.width / 2; i < originalLadderTexture.height + originalLadderTexture.width / 2; i++)
            {
                for (int j = 0; j < originalLadderTexture.width; j++)
                {
                    expandedLadderTexture.SetPixel(j, i, originalLadderTexture.GetPixel(j, i - originalLadderTexture.width / 2));
                }
            }
            for (int i = originalLadderTexture.height + originalLadderTexture.width / 2; i < originalLadderTexture.height + 5 * originalLadderTexture.width / 4; i++)
            {
                for (int j = 0; j < originalLadderTexture.width; j++)
                {
                    expandedLadderTexture.SetPixel(j, i, Color.clear);
                }
            }
            expandedLadderTexture.Apply();

            //createLadderArray();
            createLadderThreads();
        }

        public static void CreateLadderArray()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            int topBoundary = originalLadderTexture.height;
            float stepSize = (float)topBoundary / (float)graduation;
            Sprite[] thisLadderSpriteArray = new Sprite[graduation + 1];

            for (int i = 0; i < graduation + 1; i++)
            {
                Color[] pixelArray;
                if (topBoundary < i * stepSize)
                {
                    pixelArray = expandedLadderTexture.GetPixels(0, topBoundary, originalLadderTexture.width, originalLadderTexture.width);
                }
                else
                {
                    pixelArray = expandedLadderTexture.GetPixels(0, Mathf.RoundToInt(i * stepSize), originalLadderTexture.width, originalLadderTexture.width);
                }
                Texture2D m2Texture = new Texture2D(originalLadderTexture.width, originalLadderTexture.width);
                m2Texture.SetPixels(pixelArray);
                m2Texture.Apply();
                Sprite thisSprite = Sprite.Create(m2Texture, new Rect(0.0f, 0.0f, originalLadderTexture.width, originalLadderTexture.width), new Vector2(0.5f, 0.5f), 100.0f);
                thisLadderSpriteArray[i] = thisSprite;
            }

            ladderSpriteArray = thisLadderSpriteArray;

            watch.Stop();
            Logger.Log("CreateLadderArray took: " + watch.ElapsedMilliseconds.ToString() + "ms!");
        }

        // threadWorkLoad must be a factor of 180
        public const int threadWorkLoad = 6;
        public static ConcurrentDictionary<int, Texture2D> mySpecialTextureArray = new ConcurrentDictionary<int, Texture2D>();
        public static Thread[] pixelThreads  = new Thread[180 / threadWorkLoad];
        public static Thread[] spriteThreads = new Thread[180 / threadWorkLoad];

        public static void createLadderPixels(object startInt)
        {
            int topBoundary = originalLadderTexture.height;
            float stepSize = (float)topBoundary / (float)graduation;
            for (int i = (int)startInt; i < (int)startInt + threadWorkLoad; i++)
            {
                Color[] desePixels = expandedLadderTexture.GetPixels(0, Mathf.RoundToInt(i * stepSize), originalLadderTexture.width, originalLadderTexture.width);
                mySpecialTextureArray[i].SetPixels(desePixels);
            }
        }

        public static void createLadderSprite(object indexObject)
        {
            int index = (int)indexObject;
            for (int i = index; i < index + threadWorkLoad; i++)
            {
                Sprite temp = Sprite.Create(mySpecialTextureArray[i], new Rect(0.0f, 0.0f, originalLadderTexture.width, originalLadderTexture.width), new Vector2(0.5f, 0.5f), 100.0f);
                ladderSpriteArray[i] = temp;
            }
        }

        public static void createLadderThreads()
        {
            /*
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            */

            for (int i = 0; i < 180; i++)
            {
                Texture2D m2Texture = new Texture2D(originalLadderTexture.width, originalLadderTexture.width);
                mySpecialTextureArray[i] = m2Texture;
            }
            for (int i = 0; i < 180; i += threadWorkLoad)
            {
                Thread t = new Thread(createLadderPixels);
                t.Start(i);
                pixelThreads[i / threadWorkLoad] = t;
            }
            foreach (Thread th in pixelThreads)
            {
                th.Join();
            }
            for (int i = 0; i < 180; i++)
            {
                mySpecialTextureArray[i].Apply();
            }
            for (int i = 0; i < 180; i += threadWorkLoad)
            {
                Thread t = new Thread(createLadderSprite);
                t.Start(i);
                spriteThreads[i / threadWorkLoad] = t;
            }

            // add back in number 180...
            Texture2D myTexture = new Texture2D(originalLadderTexture.width, originalLadderTexture.width);
            Color[] desePixels = expandedLadderTexture.GetPixels(0, originalLadderTexture.height, originalLadderTexture.width, originalLadderTexture.width);
            myTexture.SetPixels(desePixels);
            myTexture.Apply();
            ladderSpriteArray[180] = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, originalLadderTexture.width, originalLadderTexture.width), new Vector2(0.5f, 0.5f), 100.0f);

            /*
            watch.Stop();
            Logger.Log("Create Ladder (partially threaded) took " + watch.ElapsedMilliseconds + "ms!");
            */
        }

        public static void createAttitudeIndicator(VehicleType myVT)
        {
            AttitudeIndicatorPatcher.currentVehicle = myVT;

            AttitudeIndicatorFrame = new GameObject("AttitudeIndicatorFrame");
            SpriteRenderer frameSpriteRenderer = AttitudeIndicatorFrame.AddComponent<SpriteRenderer>();
            frameSpriteRenderer.sprite = AttitudeIndicatorFrameSprite;
            frameSpriteRenderer.sortingOrder = 101;

            AttitudeIndicatorScreen = new GameObject("AttitudeIndicatorScreen");
            SpriteRenderer screenSpriteRenderer = AttitudeIndicatorScreen.AddComponent<SpriteRenderer>();
            screenSpriteRenderer.sprite = AttitudeIndicatorScreenSprite;
            screenSpriteRenderer.sortingOrder = 99;

            AttitudeIndicatorLadder = new GameObject("AttitudeIndicatorLadder");
            ladderSpriteRenderer = AttitudeIndicatorLadder.AddComponent<SpriteRenderer>();

#if SUBNAUTICA
            if (myVT == VehicleType.Seamoth)
            {
                frameSpriteRenderer.color = new Color(0.5383f, 0.5383f, 0.5383f);
                screenSpriteRenderer.color = new Color(0.8371f, 0.8371f, 0.8371f);
            }
            else if(myVT == VehicleType.Cyclops)
            {
                frameSpriteRenderer.color = new Color(0.6693f, 0.6693f, 0.6693f);
                screenSpriteRenderer.color = new Color(1, 1, 1);
            }
#elif BELOWZERO
            if (myVT == VehicleType.Seatruck)
            {
                frameSpriteRenderer.color = new Color(0.7197f, 0.7197f, 0.7197f);
                screenSpriteRenderer.color = new Color(1, 1, 1);
            }
            else if (myVT == VehicleType.Snowfox)
            {
                frameSpriteRenderer.color = new Color(1, 1, 1);
                screenSpriteRenderer.color = new Color(1, 1, 1);
            }
#endif
        }

        public static void getLadder()
        {
            Transform playerT = Player.main.transform;

            // adjust pitch to be in (0,gradutation);
            float playerRawPitch = playerT.eulerAngles.x;
            //AdjustedPitch in (-1,1)
            float playerAdjustedPitch = ((playerRawPitch >= 270f) ? 360f - playerRawPitch : playerRawPitch * -1f) / 90f;
            //FinalPitch in [0, graduation]
            float playerFinalPitch = playerAdjustedPitch * (graduation / 2) + (graduation / 2);

            int ladderSpriteIndex = Mathf.RoundToInt(playerFinalPitch);
            ladderSpriteRenderer.sprite = ladderSpriteArray[ladderSpriteIndex];
            ladderSpriteRenderer.sortingOrder = 100;
        }

        public static void updateAttitudeIndicator(VehicleType vType, bool shouldUpdateLadder)
        {
            Transform playerT = Player.main.transform;

            if (shouldUpdateLadder)
            {
                getLadder();
            }

            // make the instrument face the player
            AttitudeIndicatorFrame.transform.rotation = playerT.rotation;
            AttitudeIndicatorScreen.transform.eulerAngles = Vector3.zero;
            AttitudeIndicatorScreen.transform.forward = playerT.forward;
            AttitudeIndicatorLadder.transform.rotation = playerT.rotation;

            switch (vType)
            {
#if SUBNAUTICA
                case VehicleType.Seamoth:
                    float mySx = AttitudeIndicatorPatcher.SubnauticaConfig.Sx;
                    float mySy = AttitudeIndicatorPatcher.SubnauticaConfig.Sy;
                    float mySscale = AttitudeIndicatorPatcher.SubnauticaConfig.Sscale;

                    AttitudeIndicatorFrame.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * mySx + playerT.up * mySy;
                    AttitudeIndicatorScreen.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * mySx + playerT.up * mySy;
                    AttitudeIndicatorLadder.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * mySx + playerT.up * mySy;

                    AttitudeIndicatorFrame.transform.localScale = new Vector3(mySscale, mySscale, 1);
                    AttitudeIndicatorScreen.transform.localScale = new Vector3(mySscale, mySscale, 1);
                    AttitudeIndicatorLadder.transform.localScale = new Vector3(mySscale, mySscale, 1);
                    break;
                case VehicleType.Cyclops:
                    float myCx = AttitudeIndicatorPatcher.SubnauticaConfig.Cx;
                    float myCy = AttitudeIndicatorPatcher.SubnauticaConfig.Cy;
                    float myCscale = AttitudeIndicatorPatcher.SubnauticaConfig.Cscale;

                    AttitudeIndicatorFrame.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myCx + playerT.up * myCy;
                    AttitudeIndicatorScreen.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myCx + playerT.up * myCy;
                    AttitudeIndicatorLadder.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myCx + playerT.up * myCy;

                    AttitudeIndicatorFrame.transform.localScale = new Vector3(myCscale, myCscale, 1);
                    AttitudeIndicatorScreen.transform.localScale = new Vector3(myCscale, myCscale, 1);
                    AttitudeIndicatorLadder.transform.localScale = new Vector3(myCscale, myCscale, 1);
                    break;
#elif BELOWZERO
                case VehicleType.Seatruck:
                    float myTX = AttitudeIndicatorPatcher.BelowZeroConfig.Tx;
                    float myTY = AttitudeIndicatorPatcher.BelowZeroConfig.Ty;
                    float myTScale = AttitudeIndicatorPatcher.BelowZeroConfig.Tscale;

                    AttitudeIndicatorFrame.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myTX + playerT.up * myTY;
                    AttitudeIndicatorScreen.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myTX + playerT.up * myTY;
                    AttitudeIndicatorLadder.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myTX + playerT.up * myTY;

                    AttitudeIndicatorFrame.transform.localScale = new Vector3(myTScale, myTScale, 1);
                    AttitudeIndicatorScreen.transform.localScale = new Vector3(myTScale, myTScale, 1);
                    AttitudeIndicatorLadder.transform.localScale = new Vector3(myTScale, myTScale, 1);
                    break;
                case VehicleType.Snowfox:
                    float myFX = AttitudeIndicatorPatcher.BelowZeroConfig.Fx;
                    float myFY = AttitudeIndicatorPatcher.BelowZeroConfig.Fy;
                    float myFScale = AttitudeIndicatorPatcher.BelowZeroConfig.Fscale;

                    AttitudeIndicatorFrame.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myFX + playerT.up * myFY;
                    AttitudeIndicatorScreen.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myFX + playerT.up * myFY;
                    AttitudeIndicatorLadder.transform.position = playerT.position + playerT.forward * 0.16f + playerT.right * myFX + playerT.up * myFY;

                    AttitudeIndicatorFrame.transform.localScale = new Vector3(myFScale, myFScale, 1);
                    AttitudeIndicatorScreen.transform.localScale = new Vector3(myFScale, myFScale, 1);
                    AttitudeIndicatorLadder.transform.localScale = new Vector3(myFScale, myFScale, 1);
                    break;
#endif
            }
        }
        public static void killAttitudeIndicator()
        {
            GameObject.Destroy(AttitudeIndicatorFrame);
            GameObject.Destroy(AttitudeIndicatorScreen);
            GameObject.Destroy(AttitudeIndicatorLadder);
        }
    }
}

