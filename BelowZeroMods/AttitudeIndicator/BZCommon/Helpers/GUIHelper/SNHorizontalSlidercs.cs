using UnityEngine;
using UWE;

namespace BZCommon.Helpers.GUIHelper
{
    public static class SNHorizontalSlider
    {
        public static void CreateHorizontalSlider(Rect rect, ref float sliderValue, float leftValue, float rightValue, string label, Event<object> onSliderValueChangedEvent)
        {
            Vector2 labelSize = SNStyles.GetGuiItemStyle(GuiItemType.LABEL).CalcSize(new GUIContent(label));

            GUI.Label(new Rect(rect.x, rect.y + 5, labelSize.x, labelSize.y), label, SNStyles.GetGuiItemStyle(GuiItemType.LABEL, textAnchor: TextAnchor.MiddleLeft));

            GUI.Label(new Rect(rect.x + labelSize.x + 5, rect.y + 5, rect.width - labelSize.x, labelSize.y), string.Format("{0:#.##}", sliderValue), SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, textAnchor: TextAnchor.MiddleLeft));

            object value = GUI.HorizontalSlider(new Rect(rect.x, rect.y + labelSize.y + 5, rect.width, 10), sliderValue, leftValue, rightValue);

            if ((float)value != sliderValue)
            {
                lock (value)
                {
                    onSliderValueChangedEvent.Trigger(value);
                }
            }
        }
    }
}
