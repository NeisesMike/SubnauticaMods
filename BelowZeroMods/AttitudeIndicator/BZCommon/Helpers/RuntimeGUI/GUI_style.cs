using static BZCommon.Helpers.GraphicsHelper;
using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public static class GUI_style
    {
        private static GUISkin SimpleSkin;
        private static GUIStyle TitleBox;        
        
        private static GUIStyle TitleButton;
        private static GUIStyle ToggleButton;
        private static GUIStyle Tab;
        private static GUIStyle Label;
        private static GUIStyle TextField;
        private static GUIStyle TextArea;        
        private static GUIStyle DropDown;             

        private static Texture2D normalTexture;
        private static Texture2D hoverTexture;
        private static Texture2D activeTexture;        
        private static Texture2D clearTexture;
        private static Texture2D grayTexture;
        private static Texture2D darkTexture;
        private static Texture2D lightTexture;

        private static readonly Color32 clearColor = new Color32(0, 0, 0, 0);

        private static bool isInitStyles = false;

        private static bool CreateBackgroundTextures()
        {           
            clearTexture = MakeTex(12, 12, new Color32(0, 0, 0, 0));            
            normalTexture = GUI.skin.box.normal.background;            
            hoverTexture = CreateModifiedTextureClone(normalTexture, new Color32(48, 117, 255, 20), 0, ColorMode.Change, AlphaMode.Negative);
            //hoverTexture = CreateBoxTextureClone(new Color32(50, 80, 150, 0));
            //activeTexture = CreateBoxTextureClone(new Color32(48, 117, 202, 225), false);
            activeTexture = CreateModifiedTextureClone(normalTexture, new Color32(48, 117, 255, 20), 0, ColorMode.Change, AlphaMode.Positive);
            grayTexture = CreateModifiedTextureClone(normalTexture, new Color32(100, 100, 100, 0), 0, ColorMode.Change, AlphaMode.Ignore);            
            lightTexture = CreateModifiedTextureClone(normalTexture, new Color32(0, 0, 0, 20), 0, ColorMode.Ignore, AlphaMode.Negative);
            darkTexture = CreateModifiedTextureClone(normalTexture, new Color32(0, 0, 0, 30), 0, ColorMode.Ignore, AlphaMode.Positive);
            return true;
        }
        
        public static GUISkin GetSkin()
        {
            if (!isInitStyles)
                isInitStyles = SetGUIStyles();

            return SimpleSkin;
        }
        
        public static void CreateSkin()
        {
            SimpleSkin = ScriptableObject.CreateInstance<GUISkin>();

            SimpleSkin.name = "SimpleSkin";
            SimpleSkin.box = new GUIStyle(GUI.skin.box);
            SimpleSkin.button = new GUIStyle(GUI.skin.button);
            SimpleSkin.label = new GUIStyle(GUI.skin.label);
            SimpleSkin.scrollView = new GUIStyle(GUI.skin.scrollView);
            SimpleSkin.textArea = new GUIStyle(GUI.skin.textArea);
            SimpleSkin.textField = new GUIStyle(GUI.skin.textField);
            SimpleSkin.toggle = new GUIStyle(GUI.skin.toggle);
            SimpleSkin.window = new GUIStyle(GUI.skin.window);
            SimpleSkin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
            SimpleSkin.horizontalScrollbarLeftButton = new GUIStyle(GUI.skin.horizontalScrollbarLeftButton);
            SimpleSkin.horizontalScrollbarRightButton = new GUIStyle(GUI.skin.horizontalScrollbarRightButton);
            SimpleSkin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
            SimpleSkin.horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
            SimpleSkin.horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
            SimpleSkin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
            SimpleSkin.verticalScrollbarDownButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
            SimpleSkin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
            SimpleSkin.verticalScrollbarUpButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
            SimpleSkin.verticalSlider = new GUIStyle(GUI.skin.verticalSlider);
            SimpleSkin.verticalSliderThumb = new GUIStyle(GUI.skin.verticalSliderThumb);
            SimpleSkin.customStyles = GUI.skin.customStyles.Clone() as GUIStyle[];
            SimpleSkin.hideFlags = GUI.skin.hideFlags;

            SimpleSkin.settings.cursorColor = GUI.skin.settings.cursorColor;
            SimpleSkin.settings.cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
            SimpleSkin.settings.doubleClickSelectsWord = GUI.skin.settings.doubleClickSelectsWord;
            SimpleSkin.settings.tripleClickSelectsLine = GUI.skin.settings.tripleClickSelectsLine;
            SimpleSkin.settings.selectionColor = GUI.skin.settings.selectionColor;            
            SimpleSkin.font = GUI.skin.font;
        }

        /// <summary>
        /// Static GUIStyle initialization.
        /// This is minimize garbage from using GUIStyles.
        /// Don't use 'new GUIStyle()' call inside 'OnGUI'!
        /// </summary>
        private static bool SetGUIStyles()
        {
            CreateBackgroundTextures();

            CreateSkin();
                        
            ToggleButton = new GUIStyle(GUI.skin.button);
            Tab = new GUIStyle(GUI.skin.button);
            Label = SimpleSkin.label;
            TextField = SimpleSkin.textField;
            TextArea = SimpleSkin.textArea;
            TitleBox = new GUIStyle(GUI.skin.box);
            DropDown = SimpleSkin.box;

            SimpleSkin.window.normal.background = darkTexture;
            SimpleSkin.window.onNormal.background = darkTexture;
            SimpleSkin.window.SetBorderAndMargin();

            SimpleSkin.button.normal.background = lightTexture;
            SimpleSkin.button.hover.background = hoverTexture;
            SimpleSkin.button.active.background = activeTexture;
            //SimpleSkin.button.margin = new RectOffset(0, 0, 0, 0);
            SimpleSkin.button.border = new RectOffset(6, 6, 6, 6);
            SimpleSkin.button.padding = new RectOffset(4, 4, 3, 3);

            //SimpleSkin.button.SetBorderAndMargin();

            SimpleSkin.verticalScrollbar.normal.background = normalTexture;
            SimpleSkin.verticalScrollbar.SetBorderAndMargin();

            SimpleSkin.verticalScrollbarThumb.normal.background = grayTexture;
            SimpleSkin.verticalScrollbarThumb.hover.background = hoverTexture;
            SimpleSkin.verticalScrollbarThumb.active.background = activeTexture;
            //SimpleSkin.verticalScrollbarThumb.SetBorderAndMargin();

            SimpleSkin.horizontalScrollbar.normal.background = normalTexture;
            SimpleSkin.horizontalScrollbar.SetBorderAndMargin();            

            SimpleSkin.horizontalScrollbarThumb.normal.background = grayTexture;
            SimpleSkin.horizontalScrollbarThumb.hover.background = hoverTexture;
            SimpleSkin.horizontalScrollbarThumb.active.background = activeTexture;
            SimpleSkin.horizontalScrollbarThumb.SetBorderAndMargin();

            SimpleSkin.horizontalSlider.normal.background = normalTexture;
            //SimpleSkin.horizontalSlider.SetBorderAndMargin();

            SimpleSkin.horizontalSlider.fixedHeight = 18;
            SimpleSkin.horizontalSlider.stretchHeight = true;
            //SimpleSkin.horizontalSlider.margin = new RectOffset(0, 0, 0, 0);
            SimpleSkin.horizontalSlider.border = new RectOffset(6, 6, 6, 6);
            SimpleSkin.horizontalSlider.padding = new RectOffset(0, 0, 2, 0);

            SimpleSkin.horizontalSliderThumb.normal.background = grayTexture;
            SimpleSkin.horizontalSliderThumb.hover.background = hoverTexture;
            SimpleSkin.horizontalSliderThumb.active.background = activeTexture;
            //SimpleSkin.horizontalSliderThumb.SetBorderAndMargin();

            SimpleSkin.horizontalSliderThumb.fixedHeight = 12;
            SimpleSkin.horizontalSliderThumb.stretchHeight = true;
            SimpleSkin.horizontalSliderThumb.margin = new RectOffset(0, 0, 0, 0);
            SimpleSkin.horizontalSliderThumb.border = new RectOffset(6, 6, 6, 6);
            SimpleSkin.horizontalSliderThumb.padding = new RectOffset(0, 0, 0, 0);
            SimpleSkin.horizontalSliderThumb.fixedWidth = 40;
            SimpleSkin.horizontalSliderThumb.stretchWidth = true;

            TitleButton = new GUIStyle(GUI.skin.button);
            TitleButton.normal.background = clearTexture;
            TitleButton.hover.background = hoverTexture;
            TitleButton.active.background = activeTexture;
            TitleButton.SetBorderAndMargin();
            
            TitleBox.normal.background = normalTexture;            

            return true;
        }
        
        private static void SetBorderAndMargin(this GUIStyle gUIStyle)
        {
            gUIStyle.border = GUI.skin.box.border;            
            gUIStyle.margin = GUI.skin.box.margin;

            Debug.Log($"button margin: {GUI.skin.button.margin}");
            Debug.Log($"button border: {GUI.skin.button.border}");
            Debug.Log($"button padding: {GUI.skin.button.padding}");

            Debug.Log($"box margin: {GUI.skin.box.margin}");
            Debug.Log($"box border: {GUI.skin.box.border}");            
            Debug.Log($"box padding: {GUI.skin.box.padding}");

            Debug.Log($"hSlider margin: {GUI.skin.horizontalSlider.margin}");
            Debug.Log($"hSlider border: {GUI.skin.horizontalSlider.border}");            
            Debug.Log($"hSlider padding: {GUI.skin.horizontalSlider.padding}");

            Debug.Log($"hSliderThumb margin: {GUI.skin.horizontalSliderThumb.margin}");
            Debug.Log($"hSliderThumb border: {GUI.skin.horizontalSliderThumb.border}");            
            Debug.Log($"hSliderThumb padding: {GUI.skin.horizontalSliderThumb.padding}");
        }
               
        public static GUIStyle GetGuiStyle
            (
            GUI_Item_Type type,
            GUI_Color colorNormal = GUI_Color.White,
            GUI_Color colorHover = GUI_Color.White,
            GUI_Color colorActive = GUI_Color.White,
            TextAnchor align = TextAnchor.MiddleCenter,
            FontStyle fontStyle = FontStyle.Normal,
            bool wordWrap = false,
            TextClipping clipping = TextClipping.Overflow
            )
        {
            if (!isInitStyles)
                isInitStyles = SetGUIStyles();

            switch (type)
            {
                case GUI_Item_Type.TITLEBUTTON:
                    TitleButton.normal.textColor = GetGuiColor(colorNormal);
                    TitleButton.hover.textColor = GetGuiColor(colorHover);
                    TitleButton.active.textColor = GetGuiColor(colorActive);
                    TitleButton.fontStyle = fontStyle;
                    TitleButton.alignment = align;
                    TitleButton.wordWrap = wordWrap;
                    TitleButton.clipping = clipping;
                    return TitleButton;

                case GUI_Item_Type.TITLETEXT:
                    SimpleSkin.label.normal.textColor = GetGuiColor(colorNormal);
                    SimpleSkin.label.fontStyle = fontStyle;
                    SimpleSkin.label.alignment = align;
                    SimpleSkin.label.wordWrap = wordWrap;
                    SimpleSkin.label.clipping = clipping;
                    return SimpleSkin.label;

                case GUI_Item_Type.NORMALBUTTON:                
                    SimpleSkin.button.normal.textColor = GetGuiColor(colorNormal);
                    SimpleSkin.button.hover.textColor = GetGuiColor(colorHover);
                    SimpleSkin.button.active.textColor = GetGuiColor(colorActive);
                    SimpleSkin.button.fontStyle = fontStyle;
                    SimpleSkin.button.alignment = align;
                    SimpleSkin.button.wordWrap = wordWrap;
                    SimpleSkin.button.clipping = clipping;
                    return SimpleSkin.button;

                case GUI_Item_Type.TOGGLEBUTTON:
                    ToggleButton.normal.textColor = GetGuiColor(colorNormal);
                    ToggleButton.hover.textColor = GetGuiColor(colorHover);
                    ToggleButton.active.textColor = GetGuiColor(colorActive);
                    ToggleButton.fontStyle = fontStyle;
                    ToggleButton.alignment = align;
                    ToggleButton.wordWrap = wordWrap;
                    ToggleButton.clipping = clipping;
                    return ToggleButton;

                case GUI_Item_Type.TAB:
                    Tab.normal.textColor = GetGuiColor(colorNormal);
                    Tab.hover.textColor = GetGuiColor(colorHover);
                    Tab.active.textColor = GetGuiColor(colorActive);
                    Tab.fontStyle = fontStyle;
                    Tab.alignment = align;
                    Tab.wordWrap = wordWrap;
                    Tab.clipping = clipping;
                    return Tab;

                case GUI_Item_Type.LABEL:
                    Label.normal.textColor = GetGuiColor(colorNormal);                    
                    Label.fontStyle = fontStyle;
                    Label.alignment = align;
                    Label.wordWrap = wordWrap;
                    Label.clipping = clipping;
                    return Label;

                case GUI_Item_Type.TEXTFIELD:
                    TextField.normal.textColor = GetGuiColor(colorNormal);                    
                    TextField.fontStyle = fontStyle;
                    TextField.alignment = align;
                    TextField.wordWrap = wordWrap;
                    TextField.clipping = clipping;
                    return TextField;

                case GUI_Item_Type.TEXTAREA:
                    TextArea.normal.textColor = GetGuiColor(colorNormal);
                    TextArea.fontStyle = fontStyle;
                    TextArea.alignment = align;
                    TextArea.wordWrap = wordWrap;
                    TextArea.clipping = clipping;
                    return TextArea;

                case GUI_Item_Type.TITLEBOX:
                    return TitleBox;

                case GUI_Item_Type.DROPDOWN:
                    return DropDown;                
            }
            
            return Label;
        }

        public static GUIStyle GetGuiStyle(GUI_item item)
        {
            if (!isInitStyles)
                isInitStyles = SetGUIStyles();

            switch (item.ContentType)
            {
                case GUI_Item_Type.NORMALBUTTON:
                    SimpleSkin.button.normal.textColor = GetGuiColor(item.TextColor.Normal);
                    SimpleSkin.button.hover.textColor = GetGuiColor(item.TextColor.Hover);
                    SimpleSkin.button.active.textColor = GetGuiColor(item.TextColor.Active);
                    SimpleSkin.button.fontStyle = item.FontStyle;
                    SimpleSkin.button.alignment = item.Align;
                    SimpleSkin.button.wordWrap = item.WordWrap;
                    SimpleSkin.button.clipping = item.Clipping;
                    return SimpleSkin.button;

                case GUI_Item_Type.TOGGLEBUTTON:
                case GUI_Item_Type.TAB:
                    if (item.State == GUI_Item_State.PRESSED)
                    {
                        Tab.normal.background = normalTexture;
                        Tab.hover.background = hoverTexture;
                        Tab.active.background = activeTexture;
                        Tab.normal.textColor = GetGuiColor(item.TextColor.Active);
                        Tab.hover.textColor = GetGuiColor(item.TextColor.Hover);
                        Tab.active.textColor = GetGuiColor(item.TextColor.Active);
                    }
                    else
                    {
                        Tab.normal.background = normalTexture;
                        Tab.hover.background = hoverTexture;
                        Tab.active.background = activeTexture;
                        Tab.normal.textColor = GetGuiColor(item.TextColor.Normal);
                        Tab.hover.textColor = GetGuiColor(item.TextColor.Hover);
                        Tab.active.textColor = GetGuiColor(item.TextColor.Active);
                    }
                    
                    Tab.fontStyle = item.FontStyle;
                    Tab.alignment = item.Align;
                    Tab.wordWrap = item.WordWrap;
                    Tab.clipping = item.Clipping;
                    return Tab;

                case GUI_Item_Type.LABEL:
                    Label.normal.textColor = GetGuiColor(item.TextColor.Normal);
                    Label.fontStyle = item.FontStyle;
                    Label.alignment = item.Align;
                    Label.wordWrap = item.WordWrap;
                    Label.clipping = item.Clipping;
                    return Label;

                case GUI_Item_Type.GROUPLABEL:
                    Label.normal.textColor = GetGuiColor(item.TextColor.Normal);
                    Label.fontStyle = item.FontStyle;
                    Label.alignment = item.Align;
                    Label.wordWrap = item.WordWrap;
                    Label.clipping = item.Clipping;
                    return Label;

                case GUI_Item_Type.TEXTFIELD:
                    TextField.normal.textColor = GetGuiColor(item.TextColor.Normal);
                    TextField.fontStyle = item.FontStyle;
                    TextField.alignment = item.Align;
                    TextField.wordWrap = item.WordWrap;
                    TextField.clipping = item.Clipping;
                    return TextField;

                case GUI_Item_Type.TEXTAREA:
                    TextArea.normal.textColor = GetGuiColor(item.TextColor.Normal);
                    TextArea.fontStyle = item.FontStyle;
                    TextArea.alignment = item.Align;
                    TextArea.wordWrap = item.WordWrap;
                    TextArea.clipping = item.Clipping;
                    return TextArea;                

                case GUI_Item_Type.DROPDOWN:
                    return DropDown;
            }

            return Label;
        }

        public static Color GetGuiColor(GUI_Color color)
        {
            switch (color)
            {
                case GUI_Color.Black:
                    return Color.black;
                case GUI_Color.Blue:
                    return Color.blue;
                case GUI_Color.Clear:
                    return Color.clear;
                case GUI_Color.Cyan:
                    return Color.cyan;
                case GUI_Color.Green:
                    return Color.green;
                case GUI_Color.Gray:
                    return Color.gray;
                case GUI_Color.Grey:
                    return Color.grey;
                case GUI_Color.Magenta:
                    return Color.magenta;
                case GUI_Color.Red:
                    return Color.red;
                case GUI_Color.White:
                    return Color.white;
                case GUI_Color.Yellow:
                    return Color.yellow;
                default:
                    break;
            }

            return Color.white;
        }

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

        private static Texture2D CreateModifiedTextureClone(Texture2D originalTexture, Color32 newColor, byte searchForRed, ColorMode colorMode, AlphaMode alphaMode)
        {   
            Texture2D tempTexture = CreateRWTextureFromNonReadableTexture(originalTexture);
            int tWidth = normalTexture.width;
            int tHeight = normalTexture.height;

            var oldPixels = tempTexture.GetPixels32();

            Color[] newPixels = new Color[tWidth * tHeight];

            byte red = 0;
            byte green = 0;
            byte blue = 0;
            byte alpha = 0;

            for (int i = 0; i < oldPixels.Length; i++)
            {
                if (oldPixels[i].r == searchForRed)
                {
                    switch (colorMode)
                    {
                        case ColorMode.Change:
                            red = newColor.r;
                            green = newColor.g;
                            blue = newColor.b;
                            break;

                        case ColorMode.Ignore:
                            red = oldPixels[i].r;
                            green = oldPixels[i].g;
                            blue = oldPixels[i].b;
                            break;

                        case ColorMode.Negative:
                            int redNegative = oldPixels[i].r - newColor.r;
                            red = redNegative < 0 ? byte.MinValue : (byte)redNegative;
                            int greenNegative = oldPixels[i].g - newColor.g;
                            green = greenNegative < 0 ? byte.MinValue : (byte)greenNegative;
                            int blueNegative = oldPixels[i].b - newColor.b;
                            blue = blueNegative < 0 ? byte.MinValue : (byte)blueNegative;
                            break;

                        case ColorMode.Positive:
                            int redPositive = oldPixels[i].r + newColor.r;
                            red = redPositive > 255 ? byte.MaxValue : (byte)redPositive;
                            int greenPositive = oldPixels[i].g + newColor.g;
                            green = greenPositive > 255 ? byte.MaxValue : (byte)greenPositive;
                            int bluePositive = oldPixels[i].b + newColor.b;
                            blue = bluePositive > 255 ? byte.MaxValue : (byte)bluePositive;
                            break;
                    }

                    switch (alphaMode)
                    {
                        case AlphaMode.Change:
                            alpha = newColor.a;
                            break;

                        case AlphaMode.Ignore:
                            alpha = oldPixels[i].a;
                            break;

                        case AlphaMode.Negative:
                            int alphaNegative = oldPixels[i].a - newColor.a;
                            alpha = alphaNegative < 0 ? byte.MinValue : (byte)alphaNegative;
                            break;

                        case AlphaMode.Positive:
                            int alphaPositive = oldPixels[i].a + newColor.a;
                            alpha =  alphaPositive > 255 ? byte.MaxValue : (byte)alphaPositive;
                            break;
                    }

                    newPixels[i] = new Color32(red, green, blue, alpha);
                }
                else
                {
                    newPixels[i] = oldPixels[i];
                }                                         
            }

            Texture2D result = new Texture2D(tWidth, tHeight);
            result.SetPixels(newPixels);
            result.Apply();

            return result;
        }
    }
}
