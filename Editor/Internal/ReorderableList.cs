using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Custom re-implementation of the <see cref="UnityEditorInternal.ReorderableList"/>.
    /// </summary>
    public class ReorderableList : ReorderableListBase
    {
        public delegate float ElementHeightCallbackDelegate(int index);

        public ElementHeightCallbackDelegate elementHeightCallback;


        /// <summary>
        /// Offset between a dragging handle and the real mouse position.
        /// </summary>
        private float dragOffset;

        private List<int> nonDragTargetIndices;

        private Rect middleRect;


        public ReorderableList(SerializedProperty list)
            : base(list)
        { }

        public ReorderableList(SerializedProperty list, bool draggable)
            : base(list, draggable)
        { }

        public ReorderableList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : base(list, elementLabel, draggable, hasHeader, fixedSize)
        { }

        public ReorderableList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels)
            : base(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels)
        { }

        public ReorderableList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels, bool foldable)
            : base(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable)
        { }


        protected override void DoListMiddle()
        {
            var rect = GUILayoutUtility.GetRect(0, MiddleHeight, GUILayout.ExpandWidth(true));
            DoListMiddle(rect);
        }

        protected override void DoListMiddle(Rect middleRect)
        {
            this.middleRect = middleRect;
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
            if (!IsPropertyValid || !IsExpanded || IsEmpty)
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

                if (drawEmptyCallback != null)
                {
                    drawEmptyCallback(elementContentRect);
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
                var targetIndex = GetCoveredElementIndex(draggedY);
                nonDragTargetIndices.Clear();
                for (var i = 0; i < arraySize; i++)
                {
                    if (i != Index)
                    {
                        nonDragTargetIndices.Add(i);
                    }
                }

                if (targetIndex != -1)
                {
                    nonDragTargetIndices.Insert(targetIndex, -1);
                }

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

                //calculate position of the active element
                elementY = draggedY - dragOffset;
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
        }

        protected override void OnDrag(Event currentEvent)
        {
            base.OnDrag(currentEvent);

            //if we can drag, set the hot control and start dragging (storing the offset)
            dragOffset = ((currentEvent.mousePosition.y - middleRect.y) - GetElementYOffset(Index)) / 2;
            nonDragTargetIndices = new List<int>();
        }

        protected override void Update(Event currentEvent)
        {
            base.Update(currentEvent);
        }

        protected override void OnDrop(Event currentEvent)
        {
            base.OnDrop(currentEvent);
            nonDragTargetIndices = null;
        }

        protected override float GetDraggedY(Vector2 mousePosition)
        {
            return Mathf.Clamp(mousePosition.y, middleRect.yMin + dragOffset, middleRect.yMax - (GetRowHeight(Index) - dragOffset));
        }

        protected override int GetCoveredElementIndex(float localY)
        {
            var rowYOffset = middleRect.yMin;
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

            return -1;
        }

        protected override int GetCoveredElementIndex(Vector2 mousePosition)
        {
            return middleRect.Contains(mousePosition) ? GetCoveredElementIndex(mousePosition.y) : -1;
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

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }


        public void DoList(Rect rect)
        {
            var headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            var middleRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, MiddleHeight);
            var footerRect = new Rect(rect.x, middleRect.y + middleRect.height, rect.width, FooterHeight);

            using (new ZeroIndentScope())
            {
                DoListHeader(headerRect);
                DoListMiddle(middleRect);
                DoListFooter(footerRect);
            }
        }


        public float MiddleHeight
        {
            get
            {
                var arraySize = Count;
                var middleHeight = Style.padding * 2;
                if (arraySize != 0)
                {
                    middleHeight += GetElementYOffset(arraySize - 1) + GetRowHeight(arraySize - 1);
                }

                return middleHeight;
            }
        }

        public float EntireHeight
        {
            get => MiddleHeight + HeaderHeight + FooterHeight;
        }
    }
}