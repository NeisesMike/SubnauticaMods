using System;
using UnityEngine;

namespace BZCommon.Helpers.GUIHelper
{
    public static class SNStyles
    {
        private static GUIStyle NormalButton;
        private static GUIStyle ToggleButton;
        private static GUIStyle Tab;
        private static GUIStyle Label;
        private static GUIStyle Textfield;
        private static GUIStyle Textarea;
        private static GUIStyle Box;
        private static GUIStyle Dropdown;

        private static bool isInitStyles = false;

        public static bool SetGUIStyles()
        {
            NormalButton = new GUIStyle(GUI.skin.button);
            ToggleButton = new GUIStyle(GUI.skin.button);
            Tab = new GUIStyle(GUI.skin.button);
            Label = new GUIStyle(GUI.skin.label);
            Textfield = new GUIStyle(GUI.skin.textField);
            Textarea = new GUIStyle(GUI.skin.textArea);
            Box = new GUIStyle(GUI.skin.box);
            Dropdown = new GUIStyle(GUI.skin.box);

            Texture2D backgroundTex = Box.normal.background;

            NormalButton.normal.background = backgroundTex;
            NormalButton.hover.background = backgroundTex;

            ToggleButton.normal.background = backgroundTex;
            ToggleButton.hover.background = backgroundTex;

            Tab.normal.background = backgroundTex;
            Tab.hover.background = backgroundTex;

            return true;
        }

        public static GUIStyle GetGuiItemStyle(GuiItem guiItem)
        {
            if (!isInitStyles)
                isInitStyles = SetGUIStyles();

            switch (guiItem.Type)
            {
                case GuiItemType.NORMALBUTTON:
                    NormalButton.fontStyle = guiItem.FontStyle;
                    NormalButton.alignment = guiItem.TextAnchor;

                    if (guiItem.State == GuiItemState.PRESSED)
                    {
                        NormalButton.normal.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        NormalButton.hover.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        NormalButton.active.textColor = GetGuiColor(guiItem.ItemColor.Active);
                    }
                    else
                    {
                        NormalButton.normal.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                        NormalButton.hover.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                        NormalButton.active.textColor = GetGuiColor(guiItem.ItemColor.Active);
                    }
                    return NormalButton;

                case GuiItemType.TOGGLEBUTTON:
                    ToggleButton.fontStyle = guiItem.FontStyle;
                    ToggleButton.alignment = guiItem.TextAnchor;

                    if (guiItem.State == GuiItemState.PRESSED)
                    {
                        ToggleButton.normal.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        ToggleButton.hover.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        ToggleButton.active.textColor = GetGuiColor(guiItem.ItemColor.Active);
                    }
                    else
                    {
                        ToggleButton.normal.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                        ToggleButton.hover.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                        ToggleButton.active.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                    }
                    return ToggleButton;

                case GuiItemType.TAB:
                    Tab.fontStyle = guiItem.FontStyle;
                    Tab.alignment = guiItem.TextAnchor;

                    if (guiItem.State == GuiItemState.PRESSED)
                    {
                        Tab.normal.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        Tab.hover.textColor = GetGuiColor(guiItem.ItemColor.Active);
                        Tab.active.textColor = GetGuiColor(guiItem.ItemColor.Active);
                    }
                    else
                    {
                        Tab.normal.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                        Tab.hover.textColor = GetGuiColor(guiItem.ItemColor.Hover);
                        Tab.active.textColor = GetGuiColor(guiItem.ItemColor.Active);
                    }
                    return Tab;

                case GuiItemType.TEXTFIELD:
                    Textfield.fontStyle = guiItem.FontStyle;
                    Textfield.alignment = guiItem.TextAnchor;
                    Textfield.normal.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                    Textfield.hover.textColor = GetGuiColor(guiItem.ItemColor.Hover);
                    return Textfield;

                case GuiItemType.LABEL:
                    Label.fontStyle = guiItem.FontStyle;
                    Label.alignment = guiItem.TextAnchor;
                    Label.normal.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                    Label.hover.textColor = GetGuiColor(guiItem.ItemColor.Normal);
                    return Label;
            }

            throw new Exception("Unknown error!");
        }

        public static Color GetGuiColor(GuiColor color)
        {
            switch (color)
            {
                case GuiColor.Black:
                    return Color.black;
                case GuiColor.Blue:
                    return Color.blue;
                case GuiColor.Clear:
                    return Color.clear;
                case GuiColor.Cyan:
                    return Color.cyan;
                case GuiColor.Green:
                    return Color.green;
                case GuiColor.Gray:
                    return Color.gray;
                case GuiColor.Grey:
                    return Color.grey;
                case GuiColor.Magenta:
                    return Color.magenta;
                case GuiColor.Red:
                    return Color.red;
                case GuiColor.White:
                    return Color.white;
                case GuiColor.Yellow:
                    return Color.yellow;
                default:
                    break;
            }

            return Color.white;
        }


        public static GUIStyle GetGuiItemStyle(GuiItemType type, GuiColor textColor = GuiColor.White, TextAnchor textAnchor = TextAnchor.MiddleCenter, FontStyle fontStyle = FontStyle.Normal, bool wordWrap = false)
        {
            if (!isInitStyles)
                isInitStyles = SetGUIStyles();

            switch (type)
            {
                case GuiItemType.NORMALBUTTON:
                    NormalButton.normal.textColor = GetGuiColor(textColor);
                    NormalButton.fontStyle = fontStyle;
                    NormalButton.alignment = textAnchor;
                    NormalButton.wordWrap = wordWrap;
                    return NormalButton;

                case GuiItemType.TOGGLEBUTTON:
                    return ToggleButton;

                case GuiItemType.TAB:
                    return Tab;

                case GuiItemType.LABEL:
                    Label.normal.textColor = GetGuiColor(textColor);
                    Label.fontStyle = fontStyle;
                    Label.alignment = textAnchor;
                    Label.wordWrap = wordWrap;
                    return Label;

                case GuiItemType.TEXTFIELD:
                    return Textfield;

                case GuiItemType.TEXTAREA:
                    return Textarea;

                case GuiItemType.BOX:
                    return Box;

                case GuiItemType.DROPDOWN:
                    //Dropdown.normal.background = MakeTex(10, 10, new Color(0f, 1f, 0f, 1f));                    
                    return Dropdown;
            }

            throw new Exception("Unknown error!");
        }

        /*
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        */
    }
}
