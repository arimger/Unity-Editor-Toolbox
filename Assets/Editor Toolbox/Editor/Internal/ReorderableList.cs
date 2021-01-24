using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Custom implementation of the <see cref="UnityEditorInternal.ReorderableList"/>.
    /// </summary>
    public class ReorderableList : ReorderableListBase
    {
        public delegate void DrawIndexedRectCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

        public delegate float ElementHeightCallbackDelegate(int index);


        public DrawIndexedRectCallbackDelegate drawElementHandleCallback;
        public DrawIndexedRectCallbackDelegate drawElementCallback;
        public DrawIndexedRectCallbackDelegate drawElementBackgroundCallback;

        public ElementHeightCallbackDelegate elementHeightCallback;


        private float draggedY;
        private float dragOffset;

        private List<int> nonDragTargetIndices;


        public ReorderableList(SerializedProperty list) 
            : base(list)
        { }

        public ReorderableList(SerializedProperty list, bool draggable)
            : base(list, draggable)
        { }

        public ReorderableList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : base(list, elementLabel, draggable, hasHeader, fixedSize)
        { }


        /// <summary>
        /// Starts all drawing methods for each segment.
        /// </summary>
        private void DoList(Rect headerRect, Rect midderRect, Rect footerRect)
        {
            //make sure there is no indent while drawing
            //NOTE: indentation will break some controls
            using (new ZeroIndentScope())
            {
                DoListHeader(headerRect);
                DoListMiddle(midderRect);
                DoListFooter(footerRect);
            }
        }


        protected override void DoListMiddle()
        {
            var rect = GUILayoutUtility.GetRect(0, MiddleHeight, GUILayout.ExpandWidth(true));
            DoListMiddle(rect);
        }

        protected override void DoListMiddle(Rect middleRect)
        {
            //how many elements? If none, make space for showing default line that shows no elements are present
            var arraySize = Count;

            //draw the background in repaint
            if (Event.current.type == EventType.Repaint)
            {
                if (drawMiddleBackgroundCallback != null)
                {
                    drawMiddleBackgroundCallback(middleRect);
                }
                else
                {
                    Style.middleBackgroundStyle.Draw(middleRect, false, false, false, false);
                }
            }

            //resize to the area that we want to draw our elements into
            middleRect.yMin += Style.padding;
            middleRect.yMax -= Style.padding;

            var elementY = 0.0f;
            //create the rect for individual elements in the list
            var itemElementRect = middleRect;
            var dragElementRect = middleRect;
            //the content rect is what we will actually draw into - it doesn't include the drag handle or padding
            var elementContentRect = itemElementRect;

            //handle empty or invalid list 
            if (List == null || List.isArray == false || arraySize == 0)
            {
                //there was no content, so we will draw an empty element
                itemElementRect.y = middleRect.y;
                //draw the background
                if (drawElementBackgroundCallback != null)
                {
                    drawElementBackgroundCallback(itemElementRect, -1, false, false);
                }
                else
                {
                    DrawStandardElementBackground(itemElementRect, -1, false, false, false);
                }

                elementContentRect = itemElementRect;
                elementContentRect.xMin += Style.padding;
                elementContentRect.xMax -= Style.padding;

                if (drawVoidedCallback != null)
                {
                    drawVoidedCallback(elementContentRect);
                }
                else
                {
                    EditorGUI.LabelField(elementContentRect, Style.emptyOrInvalidListContent);
                }

                return;
            }

            //if there are elements, we need to draw them - we will do
            //this differently depending on if we are dragging or not
            if (Event.current.type == EventType.Repaint && IsDragging)
            {
                //we are dragging, so we need to build the new list of target indices
                var targetIndex = CalculateRowIndex();

                nonDragTargetIndices.Clear();
                for (var i = 0; i < arraySize; i++)
                {
                    if (i != Index)
                    {
                        nonDragTargetIndices.Add(i);
                    }
                }
                nonDragTargetIndices.Insert(targetIndex, -1);

                //now draw each element in the list (excluding the active element)
                var targetSeen = false;
                for (var i = 0; i < nonDragTargetIndices.Count; i++)
                {
                    if (nonDragTargetIndices[i] == -1)
                    {
                        targetSeen = true;
                        continue;
                    }

                    var height = GetElementHeight(i, false);
                    //update the height of the element
                    itemElementRect.height = height;
                    dragElementRect.height = height;

                    //update the position of the element
                    elementY = middleRect.y + GetElementYOffset(nonDragTargetIndices[i], Index);

                    if (targetSeen)
                    {
                        elementY += GetRowHeight(Index);
                    }

                    itemElementRect.y = elementY;
                    dragElementRect.y = elementY;

                    //draw the element background
                    if (drawElementBackgroundCallback != null)
                    {
                        drawElementBackgroundCallback(itemElementRect, i, false, false);
                    }
                    else
                    {
                        DrawStandardElementBackground(itemElementRect, i, false, false, Draggable);
                    }

                    dragElementRect.xMax = dragElementRect.xMin + Style.dragAreaWidth;
                    //draw the dragging handle
                    if (drawElementHandleCallback != null)
                    {
                        drawElementHandleCallback(dragElementRect, i, false, false);
                    }
                    else
                    {
                        DrawStandardElementHandle(dragElementRect, i, false, false, Draggable);
                    }

                    elementContentRect = itemElementRect;
                    //adjust element's rect to the dragging handle
                    elementContentRect.xMin += Style.dragAreaWidth;
                    elementContentRect.xMax -= Style.padding;

                    EditorGUIUtility.labelWidth -= Style.dragAreaWidth;
                    //draw the actual element
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementContentRect, nonDragTargetIndices[i], false, false);
                    }
                    else
                    {
                        DrawStandardElement(elementContentRect, nonDragTargetIndices[i], false, false, Draggable);
                    }
                    EditorGUIUtility.labelWidth += Style.dragAreaWidth;
                }

                //finally get position of the active element
                elementY = draggedY - dragOffset + middleRect.y;
                itemElementRect.y = elementY;
                dragElementRect.y = elementY;
                //adjust rect height to the active element
                itemElementRect.height = GetElementHeight(Index);

                //actually draw the element
                if (drawElementBackgroundCallback != null)
                {
                    drawElementBackgroundCallback(itemElementRect, Index, true, true);
                }
                else
                {
                    DrawStandardElementBackground(itemElementRect, Index, true, true, Draggable);
                }

                dragElementRect.xMax = dragElementRect.xMin + Style.dragAreaWidth;
                //draw the dragging handle
                if (drawElementHandleCallback != null)
                {
                    drawElementHandleCallback(dragElementRect, Index, true, true);
                }
                else
                {
                    DrawStandardElementHandle(dragElementRect, Index, true, true, Draggable);
                }

                elementContentRect = itemElementRect;
                //adjust element's rect to the dragging handle
                elementContentRect.xMin += Style.dragAreaWidth;
                elementContentRect.xMax -= Style.padding;

                EditorGUIUtility.labelWidth -= Style.dragAreaWidth;
                //draw the active element
                if (drawElementCallback != null)
                {
                    drawElementCallback(elementContentRect, Index, true, true);
                }
                else
                {
                    DrawStandardElement(elementContentRect, Index, true, true, Draggable);
                }
                EditorGUIUtility.labelWidth += Style.dragAreaWidth;
            }
            else
            {
                //if we aren't dragging, we just draw all of the elements in order
                for (var i = 0; i < arraySize; i++)
                {
                    var isActive = (i == Index);
                    var hasFocus = (i == Index && HasKeyboardFocus());

                    var height = GetElementHeight(i);
                    //update the height of the element
                    itemElementRect.height = height;
                    dragElementRect.height = height;

                    //update the position of the element
                    elementY = middleRect.y + GetElementYOffset(i);
                    itemElementRect.y = elementY;
                    dragElementRect.y = elementY;

                    //draw the background
                    if (drawElementBackgroundCallback != null)
                    {
                        drawElementBackgroundCallback(itemElementRect, i, isActive, hasFocus);
                    }
                    else
                    {
                        DrawStandardElementBackground(itemElementRect, i, isActive, hasFocus, Draggable);
                    }

                    dragElementRect.xMax = dragElementRect.xMin + Style.dragAreaWidth;
                    //draw the dragging handle
                    if (drawElementHandleCallback != null)
                    {
                        drawElementHandleCallback(dragElementRect, i, isActive, hasFocus);
                    }
                    else
                    {
                        DrawStandardElementHandle(dragElementRect, i, isActive, hasFocus, Draggable);
                    }

                    elementContentRect = itemElementRect;
                    //adjust element's rect to the dragging handle
                    elementContentRect.xMin += Style.dragAreaWidth;
                    elementContentRect.xMax -= Style.padding;

                    EditorGUIUtility.labelWidth -= Style.dragAreaWidth;
                    //do the callback for the element
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementContentRect, i, isActive, hasFocus);
                    }
                    else
                    {
                        DrawStandardElement(elementContentRect, i, isActive, hasFocus, Draggable);
                    }
                    EditorGUIUtility.labelWidth += Style.dragAreaWidth;
                }
            }

            //handle the interaction
            DoDraggingAndSelection(middleRect);
        }


        private void UpdateDraggedY(Rect listRect)
        {
            UpdateDraggedY(listRect, Event.current.mousePosition);
        }

        private void UpdateDraggedY(Rect listRect, Vector2 mousePosition)
        {
            draggedY = Mathf.Clamp(mousePosition.y - listRect.y, dragOffset,
                listRect.height - (GetElementHeight(Index) - dragOffset));
        }

        private void DoDraggingAndSelection(Rect listRect)
        {
            var isClicked = false;
            var oldIndex = Index;
            var currentEvent = Event.current;

            switch (currentEvent.GetTypeForControl(id))
            {
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl != id)
                    {
                        return;
                    }

                    //if we have keyboard focus, arrow through the list
                    if (currentEvent.keyCode == KeyCode.DownArrow)
                    {
                        Index += 1;
                        currentEvent.Use();
                    }

                    if (currentEvent.keyCode == KeyCode.UpArrow)
                    {
                        Index -= 1;
                        currentEvent.Use();
                    }

                    if (currentEvent.keyCode == KeyCode.Escape && GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        IsDragging = false;
                        currentEvent.Use();
                    }

                    //don't allow arrowing through the ends of the list
                    Index = Mathf.Clamp(Index, 0, List.arraySize - 1);
                    break;

                case EventType.MouseDown:
                    if (!listRect.Contains(currentEvent.mousePosition) || currentEvent.button != 0)
                    {
                        break;
                    }

                    //pick the active element based on mouse position
                    Index = CalculateRowIndex(currentEvent.mousePosition.y - listRect.y);

                    if (Draggable)
                    {
                        //if we can drag, set the hot control and start dragging (storing the offset)
                        dragOffset = (currentEvent.mousePosition.y - listRect.y) - GetElementYOffset(Index);
                        UpdateDraggedY(listRect);
                        GUIUtility.hotControl = id;
                        nonDragTargetIndices = new List<int>();
                    }

                    //clicking on the list should end editing any fields
                    EditorGUIUtility.editingTextField = false;

                    SetKeyboardFocus();
                    currentEvent.Use();
                    isClicked = true;
                    break;

                case EventType.MouseDrag:
                    if (!Draggable || GUIUtility.hotControl != id)
                    {
                        break;
                    }

                    //set dragging state on first MouseDrag event after we got hotcontrol 
                    //to prevent animating elements when deleting elements by context menu
                    IsDragging = true;

                    //if we are dragging, update the position
                    UpdateDraggedY(listRect);
                    currentEvent.Use();
                    break;

                case EventType.MouseUp:
                    if (!Draggable)
                    {
                        //if mouse up was on the same index as mouse down we fire a mouse up callback (useful if for beginning renaming on mouseup)
                        if (onMouseUpCallback != null && IsMouseInActiveElement(listRect))
                        {
                            //set the keyboard control
                            onMouseUpCallback(this);
                        }

                        break;
                    }

                    //hotcontrol is only set when list is draggable
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }

                    currentEvent.Use();
                    IsDragging = false;

                    try
                    {
                        //what will be the index of this if we release?
                        var targetIndex = CalculateRowIndex();
                        if (targetIndex != Index)
                        {
                            //if the target index is different than the current index
                            if (List != null)
                            {
                                List.serializedObject.Update();
                                //reorganize the target array and move current selected element
                                List.MoveArrayElement(Index, targetIndex);

                                //unfortunately it will break any EditorGUI.BeginCheck() scope
                                //it has to be called since we edited the array property
                                List.serializedObject.ApplyModifiedProperties();
                                GUI.changed = true;
                            }

                            var oldActiveElement = Index;
                            var newActiveElement = targetIndex;

                            //update the active element, now that we've moved it
                            Index = targetIndex;
                            //give the user a callback
                            if (onDetailsCallback != null)
                            {
                                onDetailsCallback(this, oldActiveElement, newActiveElement);
                            }
                            else
                            {
                                onReorderCallback?.Invoke(this);
                            }

                            onChangedCallback?.Invoke(this);
                        }
                        else
                        {
                            onMouseUpCallback?.Invoke(this);
                        }
                    }
                    finally
                    {
                        //cleanup before we exit GUI
                        GUIUtility.hotControl = 0;
                        nonDragTargetIndices = null;
                    }
                    break;
            }

            //if the index has changed and there is a selected callback, call it
            if ((Index != oldIndex || isClicked))
            {
                onSelectCallback?.Invoke(this);
            }
        }

        private bool IsMouseInActiveElement(Rect listRect)
        {
            var mousePosition = Event.current.mousePosition;
            var mouseRowIndex = CalculateRowIndex(mousePosition.y - listRect.y);

            //check if mouse position is inside current row rect 
            return mouseRowIndex == Index && GetRowRect(mouseRowIndex, listRect).Contains(mousePosition);
        }

        private int CalculateRowIndex()
        {
            return CalculateRowIndex(draggedY);
        }

        private int CalculateRowIndex(float localY)
        {
            var rowYOffset = 0.0f;
            var itemsCount = Count;
            for (var i = 0; i < itemsCount; i++)
            {
                var height = GetRowHeight(i);
                var rowYEnd = rowYOffset + height;
                if (localY >= rowYOffset && localY < rowYEnd)
                {
                    return i;
                }

                rowYOffset += height;
            }

            return itemsCount - 1;
        }

        private float GetRowHeight(int index)
        {
            return GetElementHeight(index) + ElementSpacing;
        }

        private float GetElementHeight(int index, bool includeChildren = true)
        {
            if (elementHeightCallback != null)
            {
                return elementHeightCallback(index);
            }
            else
            {
                return EditorGUI.GetPropertyHeight(List.GetArrayElementAtIndex(index), includeChildren);
            }
        }

        private float GetElementYOffset(int index)
        {
            return GetElementYOffset(index, -1);
        }

        private float GetElementYOffset(int index, int skipIndex)
        {
            var offset = 0.0f;
            for (var i = 0; i < index; i++)
            {
                if (i != skipIndex)
                {
                    offset += GetRowHeight(i);
                }
            }

            return offset;
        }

        private float GetRowHeight()
        {
            var arraySize = Count;
            var listHeight = Style.padding * 2;
            if (arraySize != 0)
            {
                listHeight += GetElementYOffset(arraySize - 1) + GetRowHeight(arraySize - 1);
            }

            return listHeight;
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }


        public float GetHeight()
        {
            return MiddleHeight + HeaderHeight + FooterHeight;
        }


        public void DoList(Rect rect)
        {
            var headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            var middleRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, MiddleHeight);
            var footerRect = new Rect(rect.x, middleRect.y + middleRect.height, rect.width, FooterHeight);

            DoList(headerRect, middleRect, footerRect);
        }


        public float MiddleHeight
        {
            get => GetRowHeight();
        }
    }
}