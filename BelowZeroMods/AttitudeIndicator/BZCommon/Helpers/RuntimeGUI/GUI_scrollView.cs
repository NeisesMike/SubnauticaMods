using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_scrollView : GUI_group
    {
        public GUI_scrollView
            (                   
            GUI_window guiWindow,
            Group group,
            Rect drawingArea,
            GUIEventHandler eventHandler                      
            ) : base(guiWindow, group, drawingArea, eventHandler)
        {             
            _maxShowItems = group.maxShowItems;
            _verticalSpace = 2;

            CalculateClientRect();

            CalculateGroupGrid(_clientRect);

            CreateGroup();
        }
                
        private int _maxShowItems;
        private Vector2 scrollPos;
        private Rect _clientRect;        
        private Rect _drawRect;        

        private void CalculateClientRect()
        {
            _drawRect = new Rect(_drawingArea);            

            if (_groupLabel != string.Empty)
            {
                _groupLabelRect = new Rect(_drawRect.x + _group.horizontalSpace, _drawRect.y, _drawRect.width - (_group.horizontalSpace * 2), _group.itemHeight);

                _drawRect.y = _drawRect.y + (_group.itemHeight - (_verticalSpace / 2));                
            }

            if (_maxShowItems == 0)
            {
                _maxShowItems = (int)(_drawRect.height / (_group.itemHeight + _verticalSpace));
            }

            int totalRows = _group.GetTotalRows();

            if (_maxShowItems < totalRows)
            {
                _drawRect.height = _maxShowItems * (_group.itemHeight + _verticalSpace);
                _drawRect.width -= _group.horizontalSpace;
                _clientRect = new Rect(0, 0, _drawRect.width - 20, totalRows * (_group.itemHeight + _verticalSpace));                
            }
            else
            {
                _drawRect.height = totalRows * (_group.itemHeight + _verticalSpace);
                _clientRect = new Rect(0, 0, _drawRect.width, totalRows * (_group.itemHeight + _verticalSpace));
            }

            float nextYpos = _drawRect.y + _drawRect.height;

            _guiWindow.RemainDrawableArea = new Rect(_drawingArea.x, nextYpos, _drawingArea.width, _guiWindow.WindowRect.height - (_drawRect.height + _group.itemHeight));                        
        }
        
        public override void DrawGroup()
        {
            if (_groupLabel != string.Empty)
            {
                GUI.Label(_groupLabelRect, _groupLabel, GUI_style.GetGuiStyle(GUI_Item_Type.LABEL, align: TextAnchor.MiddleLeft, colorNormal: GUI_Color.White));
            }

            scrollPos = GUI.BeginScrollView(_drawRect, scrollPos, _clientRect);

            base.DrawGroup();

            GUI.EndScrollView();
        }

        public override void Refresh()
        {
            isRefresh = true;

            CalculateClientRect();            

            CalculateGroupGrid(_clientRect);

            CreateGroup();

            isRefresh = false;
        }        
    }
}
