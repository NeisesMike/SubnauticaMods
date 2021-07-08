using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_content_hSlider : GUI_content
    {
        public GUI_content_hSlider
            (
            int ID,
            string text,
            string toolTip,
            GUI_textColor textColor,
            float sliderValue,
            float leftValue,
            float rightValue,
            FontStyle fontStyle = FontStyle.Normal,
            TextAnchor textAlign = TextAnchor.MiddleCenter,
            float fixedWidth = 0f,
            float fixedHeight = 0f,
            int decimals = 0
            ) : base(ID, GUI_Item_Type.HORIZONTALSLIDER, text, toolTip, textColor, fontStyle, textAlign, GUI_Item_State.NORMAL, fixedWidth, fixedHeight)
        {
            SliderValue = sliderValue;
            LeftValue = leftValue;
            RightValue = rightValue;
            Decimals = decimals;
        }

        public float SliderValue;
        public float LeftValue;
        public float RightValue;
        public int Decimals;
    }
}
