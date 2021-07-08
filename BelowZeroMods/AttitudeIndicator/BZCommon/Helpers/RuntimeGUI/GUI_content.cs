using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_content
    {
        public readonly int ID;
        public readonly GUI_Item_Type ContentType;
        public readonly string Text;
        public readonly string ToolTip;
        public readonly GUI_textColor TextColor;
        public readonly FontStyle FontStyle;
        public readonly TextAnchor TextAlign;
        public readonly GUI_Item_State InitialState;
        public readonly float FixedWidth;
        public readonly float FixedHeight;

        public GUI_content
            (
            int ID,
            GUI_Item_Type contentType,
            string text,
            string toolTip,
            GUI_textColor textColor,
            FontStyle fontStyle = FontStyle.Normal,
            TextAnchor textAlign = TextAnchor.MiddleCenter,
            GUI_Item_State initialState = GUI_Item_State.NORMAL,
            float fixedWidth = 0f,
            float fixedHeight = 0f
            )
        {
            this.ID = ID;
            ContentType = contentType;
            Text = text;
            ToolTip = toolTip;
            TextColor = textColor;
            FontStyle = fontStyle;
            TextAlign = textAlign;
            InitialState = GUI_Item_State.NORMAL;
            FixedWidth = fixedWidth;
            FixedHeight = fixedHeight;
        }        
    }
}
