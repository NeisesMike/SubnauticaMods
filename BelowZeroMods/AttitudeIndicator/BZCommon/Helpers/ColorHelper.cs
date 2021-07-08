using System;
using UnityEngine;

namespace BZCommon.Helpers
{
    public enum COLORS
    {
        Red,
        Green,
        Blue,
        Yellow,
        White,
        Magenta,
        Cyan,
        Orange,
        Lime,
        Amethyst
    }

    public static class ColorHelper
    {
        public static Color Red => Color.red;
        public static Color Green => Color.green;
        public static Color Blue => Color.blue;
        public static Color Yellow => Color.yellow;
        public static Color White => Color.white;
        public static Color Magenta => Color.magenta;
        public static Color Cyan => Color.cyan;
        public static Color Orange => new Color32(byte.MaxValue, 146, 71, byte.MaxValue);
        public static Color Lime => new Color(0.749f, 1f, 0f, 1f);
        public static Color Amethyst => new Color(0.6f, 0.4f, 0.8f, 1f);

        public static Color[] Colors => new Color[] { Red, Green, Blue, Yellow, White, Magenta, Cyan, Orange, Lime, Amethyst };

        public static string[] ColorNames => new string[] { "Red", "Green", "Blue", "Yellow", "White", "Magenta", "Cyan", "Orange", "Lime", "Amethyst" };

        public static Color GetColor(COLORS color) => Colors[(int)color];

        public static Color GetColor(string color)
        {
            int result = Array.IndexOf(ColorNames, color);

            if (result < 0)
                return Color.white;

            return Colors[result];
        }

        public static string GetColorName(Color color)
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                if (Colors[i].Equals(color))
                    return ColorNames[i];
            }

            return null;
        }

        public static int GetColorInt(string color)
        {
            int result = Array.IndexOf(ColorNames, color);

            if (result < 0)
                return 1;

            return result;
        }

        public static int GetColorInt(Color color)
        {
            int result = Array.IndexOf(Colors, color);

            if (result < 0)
                return 1;

            return result;
        }
    }
}
