using System;
using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_group
    {
        public GUI_group
            (
            GUI_window guiWindow,
            Group group,
            Rect drawingArea,
            GUIEventHandler eventHandler            
            )
        {
            groupID = group.groupID;
            windowID = group.windowID;
            _group = group;
            _guiWindow = guiWindow;            
            _eventHandler = eventHandler;
            _drawingArea = drawingArea;
            _groupLabel = group.groupLabel ?? string.Empty;
            _columns = group.columns;
            _horizontalSpace = group.horizontalSpace;
            _verticalSpace = group.verticalSpace;
            _itemHeight = group.itemHeight;
            _layout = group.layout;

            if (group.groupType != GUI_Group_type.Scroll)
            {
                CalculateGroupGrid(_drawingArea);
                CreateGroup();
            }
        }

        public int groupID { get; }
        public int windowID { get; }

        protected readonly Group _group;
        protected readonly GUI_window _guiWindow;
        protected Rect _drawingArea;
        protected string _groupLabel;
        protected int _verticalSpace;
        protected Rect _groupLabelRect;
        protected List<GUI_item> _guiItems = new List<GUI_item>();
        protected bool isRefresh = false;
        
        private readonly GUIEventHandler _eventHandler;        
        private readonly int _columns;
        private readonly int _horizontalSpace;        
        private readonly int _itemHeight;
        private readonly GUI_Layout _layout;        
        private List<Rect> rects = new List<Rect>();        

        public float GetRowFixedWiths(List<GUI_content> row, out int contentsWithFixedWiths)
        {
            float totalFixedWith = 0;
            contentsWithFixedWiths = 0;

            foreach (GUI_content content in row)
            {
                if (content.FixedWidth != 0)
                {
                    totalFixedWith += content.FixedWidth;
                    contentsWithFixedWiths++;
                }
            }

            return totalFixedWith;
        }

        public float GetRowMaxFixedHeight(List<GUI_content> row)
        {
            float maxFixedHeight = 0;

            foreach (GUI_content content in row)
            {
                if (content.FixedHeight != 0)
                {
                    if (content.FixedHeight > maxFixedHeight)
                        maxFixedHeight = content.FixedHeight;
                }
            }

            return maxFixedHeight;
        }

        internal void CalculateGroupGrid(Rect drawRect)
        {
            if (_columns < 1)
            {
                throw new ArgumentException("The number of columns must not be less than one!");
            }           

            if (_group.groupType != GUI_Group_type.Scroll &&  _groupLabel != string.Empty)
            {
                _groupLabelRect = new Rect(drawRect.x + 2, drawRect.y, drawRect.width - 4, _itemHeight);

                drawRect.y = drawRect.y + (_itemHeight - (_verticalSpace / 2));                
            }

            List<List<GUI_content>> rowContents = _group.GetContentMatrix();

            rects.Clear();

            float yPos = _verticalSpace;

            for (int i = 0; i < rowContents.Count; i++)
            {
                List<GUI_content> item = rowContents[i];                

                float totalFixedWidth = GetRowFixedWiths(item, out int contentsWithFixedWiths);                
                float nonFixedContentWidth = (drawRect.width - (totalFixedWidth + (item.Count + 1) * _horizontalSpace)) / (item.Count - contentsWithFixedWiths);

                float maxFixedHeight = GetRowMaxFixedHeight(item);
                float newHeight = maxFixedHeight != 0 ? maxFixedHeight : _itemHeight;

                float xPos = _horizontalSpace;                

                for (int j = 0; j < item.Count; j++)
                {
                    GUI_content content = item[j];

                    float newWidth = content.FixedWidth != 0 ? content.FixedWidth : nonFixedContentWidth;                    

                    rects.Add(new Rect(drawRect.x + xPos, drawRect.y + yPos, newWidth, newHeight));
                    
                    xPos += newWidth + _horizontalSpace;
                }

                yPos += newHeight + _verticalSpace;
            }

            if (_group.groupType != GUI_Group_type.Scroll)
            {
                float nextYpos = rects.GetLast().y + _itemHeight + _verticalSpace;
                _guiWindow.RemainDrawableArea = new Rect(drawRect.x, nextYpos, drawRect.width, _guiWindow.WindowRect.height - nextYpos);
            }                      
        }

        internal void CreateGroup()
        {
            _guiItems.Clear();

            for (int i = 0; i < _group.itemsContent.Count; i++)
            {
                if (_group.itemsContent[i].ContentType == GUI_Item_Type.HORIZONTALSLIDER)
                {
                    _guiItems.Add(new GUI_horizontalSlider(_group.itemsContent[i].ID, true, rects[i], (GUI_content_hSlider)_group.itemsContent[i]));
                }
                else
                {
                    _guiItems.Add(new GUI_item(_group.itemsContent[i].ID, true, rects[i], _group.itemsContent[i]));
                }              
            }            
        }

        /// <summary>
        /// Draws the group.
        /// Call only from 'GUI_window.WindowControl'
        /// </summary>
        public virtual void DrawGroup()
        {
            if (isRefresh)
                return;

            if (_group.groupType != GUI_Group_type.Scroll && _groupLabel != string.Empty)
            {
                GUI.Label(_groupLabelRect, _groupLabel, GUI_style.GetGuiStyle(GUI_Item_Type.LABEL, align: TextAnchor.MiddleLeft, colorNormal: GUI_Color.White));
            }

            for (int i = 0; i < _guiItems.Count; ++i)
            {
                if (!_guiItems[i].Enabled)
                {
                    continue;
                }

                float result = _guiItems[i].DrawItem();

                switch (_guiItems[i].ContentType)
                {
                    case GUI_Item_Type.NORMALBUTTON:

                        if (result != -1)
                        {
                            _eventHandler(new GUI_event(windowID, groupID, _guiItems[i], (int)result));
                        }
                        
                        break;

                    case GUI_Item_Type.TOGGLEBUTTON:

                        if (result != -1)
                        {
                            if (result == 0)
                            {
                                _guiItems[i].InvertState();                                
                            }
                            
                            _eventHandler(new GUI_event(windowID, groupID, _guiItems[i], (int)result));
                        }
                        break;

                    case GUI_Item_Type.TAB:

                        if (result != -1)
                        {
                            if (result == 0)
                            {
                                SetTABState(_guiItems[i].ID);
                            }

                            _eventHandler(new GUI_event(windowID, groupID, _guiItems[i], (int)result));
                        }

                        break;

                    case GUI_Item_Type.HORIZONTALSLIDER:
                        
                        if (result != -1)
                        {
                            _eventHandler(new GUI_event(windowID, groupID, _guiItems[i], result));
                        }
                        
                        break;
                }
            }            
        }

        private void SetTABState(int id)
        {
            for (int i = 0; i < _guiItems.Count; i++)
            {
                if (_guiItems[i].ContentType == GUI_Item_Type.TAB)
                {
                    if (_guiItems[i].ID == id)
                    {
                        _guiItems[i].SetState(GUI_Item_State.PRESSED);
                    }
                    else
                    {
                        _guiItems[i].SetState(GUI_Item_State.NORMAL);
                    }
                }
            }
        }

        public GUI_item GetItemByID(int ID)
        {
            for (int i = 0; i < _guiItems.Count; i++)
            {
                if (_guiItems[i].ID == ID)
                {
                    return _guiItems[i];
                }
            }

            return null;
        }

        public virtual void Refresh()
        {
            isRefresh = true;            

            CalculateGroupGrid(_drawingArea);

            CreateGroup();

            isRefresh = false;
        }

        public void SetLabel(string text)
        {
            _groupLabel = text;
        }
    }
}
