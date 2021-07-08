using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_horizontalSlider : GUI_item
    {
        public GUI_horizontalSlider
            (
            int id,
            bool enabled,
            Rect rect,
            GUI_content_hSlider itemContent            
            ) : base (id, enabled, rect, itemContent)
        {
            _sliderContent = itemContent;

            InitSlider();
        }               
        
        private GUI_content_hSlider _sliderContent;
        private Rect _labelRect;
        private Rect _valueRect;
        private Rect _sliderRect;
        private string decimals;
        private GUIContent maxValueText;

        private Vector2 labelSize = Vector2.zero;
        private Vector2 valueSize = Vector2.zero;

        private void InitSlider()
        {
            _labelRect = new Rect(DrawingRect)
            {
                width = 0
            };
            _valueRect = new Rect(DrawingRect);
            _sliderRect = new Rect(DrawingRect);

            decimals = $"{{0:F{_sliderContent.Decimals}}}";

            maxValueText = new GUIContent(string.Format(decimals, _sliderContent.RightValue));            
        }

        public override float DrawItem()
        {
            GUI.Box(DrawingRect, string.Empty);

            if (Content.text != "")
            {
                if (labelSize == Vector2.zero)
                {
                    labelSize = GUI_style.GetGuiStyle(GUI_Item_Type.LABEL).CalcSize(Content);
                    _labelRect.x = DrawingRect.x + 5;
                    _labelRect.y = DrawingRect.y;
                    _labelRect.width = labelSize.x;
                    _labelRect.height = DrawingRect.height;                    
                }

                GUI.Label(_labelRect, Content, GUI_style.GetGuiStyle(GUI_Item_Type.LABEL, align: TextAnchor.MiddleLeft));
            }
            
            if (valueSize == Vector2.zero)
            {
                valueSize = GUI_style.GetGuiStyle(GUI_Item_Type.LABEL).CalcSize(maxValueText);
                _valueRect.x = _labelRect.x +_labelRect.width + 5;
                _valueRect.y = DrawingRect.y;
                _valueRect.width = valueSize.x;
                _valueRect.height = DrawingRect.height;

                Debug.Log($"_valueRect: {_valueRect}");

                _sliderRect.x = _valueRect.x + _valueRect.width + 5;
                _sliderRect.y = DrawingRect.y + (_labelRect.height - 19) / 2;
                _sliderRect.width = DrawingRect.width - _sliderRect.x;
                _sliderRect.height = DrawingRect.height;                
            }                                                       
            
            GUI.Label(_valueRect, string.Format(decimals, _sliderContent.SliderValue), GUI_style.GetGuiStyle(GUI_Item_Type.LABEL, colorNormal: GUI_Color.Green, align: TextAnchor.MiddleLeft));
                        
            float value = GUI.HorizontalSlider(_sliderRect, _sliderContent.SliderValue, _sliderContent.LeftValue, _sliderContent.RightValue);
            
            if (value != _sliderContent.SliderValue)
            {
                _sliderContent.SliderValue = value;
                return value;                
            }

            return -1;
        }
    }
}
