using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BZCommon.Helpers.GUIHelper
{
    public class HorizontalSlider : MonoBehaviour
    {        
        private GUIStyle _sliderBackgroundStyle;
        private GUIStyle _sliderThumbStyle;
        private float _sliderValue = 0f;
        private Texture2D _whitePixel;
        private Texture2D _blackPixel;

        void Start()
        {
            this._whitePixel = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            this._whitePixel.SetPixel(0, 0, Color.Lerp(Color.gray, Color.clear, 0.1f));
            this._whitePixel.Apply();

            this._blackPixel = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            this._blackPixel.SetPixel(0, 0, Color.Lerp(Color.black, Color.clear, 0.3f));
            this._blackPixel.Apply();

            this._sliderBackgroundStyle = new GUIStyle
            {
                padding = new RectOffset(2, 2, 2, 2)
            };
            this._sliderBackgroundStyle.normal.background = this._whitePixel;
            this._sliderBackgroundStyle.hover.background = this._whitePixel;
            this._sliderBackgroundStyle.active.background = this._whitePixel;
            this._sliderBackgroundStyle.focused.background = this._whitePixel;

            this._sliderThumbStyle = new GUIStyle
            {
                stretchHeight = true,
                fixedWidth = 20f
            };
            this._sliderThumbStyle.normal.background = this._blackPixel;
            this._sliderThumbStyle.hover.background = this._blackPixel;
            this._sliderThumbStyle.active.background = this._blackPixel;
            this._sliderThumbStyle.focused.background = this._blackPixel;
        }

        void OnGUI()
        {
            this._sliderValue = GUI.HorizontalSlider(new Rect(0, 300, 200f, 20f), this._sliderValue, 0, 1, this._sliderBackgroundStyle, this._sliderThumbStyle);
        }
        
    }
}
