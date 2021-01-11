using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Custom implementation of the <see cref="UnityEditorInternal.ReorderableList"/>.
    /// </summary>
    public class ReorderableList
    {
        public delegate float ElementCallbackDelegate(int index);

        public delegate void DrawRectCallbackDelegate(Rect rect);

        public delegate void DrawIndexedRectCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

        public delegate void DrawRelatedRectCallbackDelegate(Rect rect, ReorderableList list);

        public delegate bool CanChangeListCallbackDelegate(ReorderableList list);

        public delegate void ChangeDetailsCallbackDelegate(ReorderableList list, int oldIndex, int newIndex);

        public delegate void ChangeListCallbackDelegate(ReorderableList list);


        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawVoidedCallback;
        public DrawRectCallbackDelegate drawFooterCallback;

        public DrawRectCallbackDelegate drawHeaderBackgroundCallback;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback;
        public DrawRectCallbackDelegate drawMiddleBackgroundCallback;

        public DrawIndexedRectCallbackDelegate drawElementHandleCallback;
        public DrawIndexedRectCallbackDelegate drawElementCallback;
        public DrawIndexedRectCallbackDelegate drawElementBackgroundCallback;

        public DrawRelatedRectCallbackDelegate onAppendDropdownCallback;

        public ChangeListCallbackDelegate onAppendCallback;
        public ChangeListCallbackDelegate onRemoveCallback;
        public ChangeListCallbackDelegate onSelectCallback;

        public ChangeListCallbackDelegate onChangedCallback;
        public ChangeListCallbackDelegate onMouseUpCallback;
        public ChangeListCallbackDelegate onReorderCallback;

        public ChangeDetailsCallbackDelegate onDetailsCallback;

        public CanChangeListCallbackDelegate onCanAppendCallback;
        public CanChangeListCallbackDelegate onCanRemoveCallback;

        public ElementCallbackDelegate elementHeightCallback;


        private const string defaultLabelFormat = "{0} {1}";

        /// <summary>
        /// Hotcontrol index unique for this instance.
        /// </summary>
        private readonly int id = -1;

        private float draggedY;
        private float dragOffset;

        private float headerHeight = 18.0f;
        private float footerHeight = 20.0f;

        private List<int> nonDragTargetIndices;


        public ReorderableList(SerializedProperty list) : this(list, null, true, true, false)
        { }

        public ReorderableList(SerializedProperty list, bool draggable) : this(list, null, draggable, true, false)
        { }

        public ReorderableList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool hasFixedSize)
        {
            //validate parameters
            if (list == null || list.isArray == false)
            {
                throw new ArgumentException("List should be an Array SerializedProperty.", nameof(list));
            }

            id = GetHashCode();

            //set basic properties
            Draggable = draggable;
            HasHeader = hasHeader;
            HasFixedSize = hasFixedSize;
            ElementLabel = elementLabel;

            //ser serialized data
            List = list;
            Size = list.FindPropertyRelative("Array.size");
        }


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


        private void DoListHeader(Rect headerRect)
        {
            if (!HasHeader)
            {
                return;
            }

            //draw the background on repaint
            if (Event.current.type == EventType.Repaint)
            {
                if (drawHeaderBackgroundCallback != null)
                {
                    drawHeaderBackgroundCallback(headerRect);
                }
                else
                {
                    DrawStandardHeaderBackground(headerRect);
                }
            }

            //apply the padding to get the internal rect
            headerRect.xMin += Style.padding;
            headerRect.xMax -= Style.padding;
            headerRect.yMin -= Style.spacing / 2;
            headerRect.yMax += Style.spacing / 2;

            //perform the default or overridden callback
            if (drawHeaderCallback != null)
            {
                drawHeaderCallback(headerRect);
            }
            else
            {
                DrawStandardHeader(headerRect);
            }
        }

        private void DoListMiddle(Rect middleRect)
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
                if (drawElementBackgroundCallback == null)
                {
                    DrawStandardElementBackground(itemElementRect, -1, false, false, false);
                }
                else
                {
                    drawElementBackgroundCallback(itemElementRect, -1, false, false);
                }

                elementContentRect = itemElementRect;
                elementContentRect.xMin += Style.padding;
                elementContentRect.xMax -= Style.padding;

                if (drawVoidedCallback == null)
                {
                    EditorGUI.LabelField(elementContentRect, Style.emptyOrInvalidListContent);
                }
                else
                {
                    drawVoidedCallback(elementContentRect);
                }

                return;
            }

            //if there are elements, we need to draw them - we will do this differently depending on if we are dragging or not
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

        private void DoListFooter(Rect footerRect)
        {
            //ignore footer if list has fixed size
            if (HasFixedSize)
            {
                return;
            }

            //draw the background on repaint event
            if (Event.current.type == EventType.Repaint)
            {
                if (drawFooterBackgroundCallback != null)
                {
                    drawFooterBackgroundCallback(footerRect);
                }
                else
                {
                    DrawStandardFooterBackground(footerRect);
                }
            }

            //perform callback or the default footer
            if (drawFooterCallback != null)
            {
                drawFooterCallback(footerRect);
            }
            else
            {
                DrawStandardFooter(footerRect);
            }
        }


        private void UpdateDraggedY(Rect listRect)
        {
            draggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, dragOffset,
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
                    if (!listRect.Contains(currentEvent.mousePosition) || Event.current.button != 0)
                    {
                        break;
                    }

                    //clicking on the list should end editing any existing edits
                    EditorGUIUtility.editingTextField = false;
                    //pick the active element based on click position
                    Index = CalculateRowIndex(currentEvent.mousePosition.y - listRect.y);

                    if (Draggable)
                    {
                        //if we can drag, set the hot control and start dragging (storing the offset)
                        dragOffset = (currentEvent.mousePosition.y - listRect.y) - GetElementYOffset(Index);
                        UpdateDraggedY(listRect);
                        GUIUtility.hotControl = id;
                        nonDragTargetIndices = new List<int>();
                    }

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
            if (elementHeightCallback == null)
            {
                return EditorGUI.GetPropertyHeight(List.GetArrayElementAtIndex(index), includeChildren);
            }

            return elementHeightCallback(index);
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


        public string GetElementDefaultName(int index)
        {
            return string.Format(defaultLabelFormat, "Element", index);
        }

        public string GetElementDefinedName(int index)
        {
            return ElementLabel != null
                ? string.Format(defaultLabelFormat, ElementLabel, index) : null;
        }

        public string GetElementDisplayName(SerializedProperty element, int index)
        {
            //try to determine name using the internal API
            var elementName = element.displayName;
            var defaultName = GetElementDefaultName(index);
            if (defaultName != elementName)
            {
                return elementName;
            }

            //try to override name using customized label
            var definedName = GetElementDefinedName(index);
            if (definedName == null)
            {
                return elementName;
            }

            return definedName;
        }

        public void SetKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public bool HasKeyboardFocus()
        {
            return GUIUtility.keyboardControl == id;
        }

        public void CutKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        public float GetHeight()
        {
            return MiddleHeight + HeaderHeight + FooterHeight;
        }

        public void AppendElement()
        {
            Index = (List.arraySize += 1) - 1;
        }

        public void RemoveElement()
        {
            List.DeleteArrayElementAtIndex(Index);
            if (Index >= List.arraySize - 1)
            {
                Index = List.arraySize - 1;
            }
        }


        public void DoLayoutList()
        {
            var headerRect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            var middleRect = GUILayoutUtility.GetRect(0, MiddleHeight, GUILayout.ExpandWidth(true));
            var footerRect = GUILayoutUtility.GetRect(0, FooterHeight, GUILayout.ExpandWidth(true));

            headerRect = EditorGUI.IndentedRect(headerRect);
            middleRect = EditorGUI.IndentedRect(middleRect);
            footerRect = EditorGUI.IndentedRect(footerRect);

            DoList(headerRect, middleRect, footerRect);
        }

        public void DoList(Rect rect)
        {
            var headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            var middleRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, MiddleHeight);
            var footerRect = new Rect(rect.x, middleRect.y + middleRect.height, rect.width, FooterHeight);

            DoList(headerRect, middleRect, footerRect);
        }


        #region Methods: Default interaction/draw calls

        /// <summary>
        /// Draws the default Footer.
        /// </summary>
        public void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);

            //set rect properties from style
            var buttonWidth = Style.footerButtonWidth;
            var buttonHeight = Style.footerButtonHeight;
            var margin = Style.footerMargin;
            var padding = Style.footerPadding;

            //set proper rect for each buttons
            var appendButtonRect = new Rect(rect.xMin + margin, rect.y - padding, buttonWidth, buttonHeight);

            EditorGUI.BeginDisabledGroup(List.hasMultipleDifferentValues);
            EditorGUI.BeginDisabledGroup(onCanAppendCallback != null && !onCanAppendCallback(this));
            if (GUI.Button(appendButtonRect, onAppendDropdownCallback != null
                ? Style.iconToolbarDropContent
                : Style.iconToolbarAddContent, Style.footerButtonStyle))
            {
                if (onAppendDropdownCallback != null)
                {
                    onAppendDropdownCallback(appendButtonRect, this);
                }
                else if (onAppendCallback != null)
                {
                    onAppendCallback(this);
                }
                else
                {
                    AppendElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();

            var removeButtonRect = new Rect(rect.xMax - buttonWidth - margin, rect.y - padding, buttonWidth, buttonHeight);

            EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            if (GUI.Button(removeButtonRect, Style.iconToolbarRemoveContent, Style.footerButtonStyle))
            {
                if (onRemoveCallback != null)
                {
                    onRemoveCallback(this);
                }
                else
                {
                    RemoveElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws the default Footer background.
        /// </summary>
        public void DrawStandardFooterBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);
                Style.footerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Header.
        /// </summary>
        public void DrawStandardHeader(Rect rect)
        {
            var label = EditorGUI.BeginProperty(rect, new GUIContent(List.displayName), List);

            var diff = rect.height - Style.sizePropertyStyle.fixedHeight;
            var oldY = rect.y;

#if !UNITY_2019_3_OR_NEWER
            //adjust OY position to middle of the conent
            rect.y += diff / 2;
#endif
            //display the property label using the preprocessed name by BeginProperty method
            EditorGUI.LabelField(rect, label);

            //adjust OY position to the middle of the element row
            rect.y = oldY;
            rect.yMin += diff / 2;
            rect.yMax -= diff / 2;
            //adjust OX position and width for the size property
            rect.xMin = rect.xMax - Style.sizeAreaWidth;

            using (new EditorGUI.DisabledScope(HasFixedSize))
            {
                var property = Size;

                EditorGUI.BeginProperty(rect, Style.sizePropertyContent, property);
                EditorGUI.BeginChangeCheck();
                //cache a delayed size value using the delayed int field
                var sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, property.intValue, Style.sizePropertyStyle), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = sizeValue;
                }
                EditorGUI.EndProperty();
            }
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws the default Header background.
        /// </summary>
        /// <param name="rect"></param>
        public void DrawStandardHeaderBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.headerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Element field.
        /// </summary>
        public void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, new GUIContent(GetElementDisplayName(element, index)), element.isExpanded);
        }

        /// <summary>
        /// Draws the default dragging Handle.
        /// </summary>
        public void DrawStandardElementHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                //keep the dragging handle in the 1 row
                rect.yMax = rect.yMin + EditorGUIUtility.singleLineHeight;

                //prepare rect for the handle texture draw
                var xDiff = rect.width - Style.handleWidth;
                rect.xMin += xDiff / 2;
                rect.xMax -= xDiff / 2;

                var yDiff = rect.height - Style.handleHeight;
                rect.yMin += yDiff / 2;
                rect.yMax -= yDiff / 2;
#if UNITY_2019_3_OR_NEWER
                rect.y += Style.spacing;
#endif
                //disable (if needed) and draw the handle
                using (new EditorGUI.DisabledScope(!draggable))
                {
                    Style.dragHandleButtonStyle.Draw(rect, false, false, false, false);
                }
            }
        }

        /// <summary>                                       
        /// Draws the default Element background.
        /// </summary>
        public void DrawStandardElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (selected)
                {
                    //additional height for selection rect + shadow
                    //NOTE: shadow appears only before Unity 2019.3
#if UNITY_2019_3_OR_NEWER
                    var padding = Style.spacing;
#else
                    var padding = Style.spacing + Style.spacing / 2;
#endif
                    rect.yMax += padding / 2;
                    rect.yMin -= padding / 2;
                }

                Style.elementBackgroundStyle.Draw(rect, false, selected, selected, focused);
            }
        }

        #endregion


        public int Index
        {
            get; set;
        } = -1;

        public int Count
        {
            get
            {
                if (!List.hasMultipleDifferentValues)
                {
                    return List.arraySize;
                }

                //if we are during multi-selection
                var smallerArraySize = List.arraySize;
                foreach (var targetObject in List.serializedObject.targetObjects)
                {
                    using (var serializedObject = new SerializedObject(targetObject))
                    {
                        var property = serializedObject.FindProperty(List.propertyPath);
                        smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
                    }
                }
                //return the smallest array size
                return smallerArraySize;
            }
        }

        public bool Draggable
        {
            get; set;
        }

        public bool IsDragging
        {
            get; private set;
        }

        public bool HasFixedSize
        {
            get; private set;
        }

        public bool HasHeader
        {
            get; set;
        }

        public float HeaderHeight
        {
            get => HasHeader ? headerHeight : 0.0f;
            set => headerHeight = value;
        }

        public float MiddleHeight
        {
            get => GetRowHeight();
        }

        public float FooterHeight
        {
            get => HasFixedSize ? 0.0f : footerHeight;
            set => footerHeight = value;
        }

        /// <summary>
        /// Standard spacing between elements.
        /// </summary>
        public float ElementSpacing
        {
            get; set;
        } = 5;

        /// <summary>
        /// Standard element label name.
        /// "<see cref="ElementLabel"/> {index}"
        /// </summary>
        public string ElementLabel
        {
            get; set;
        } = defaultLabelFormat;

        /// <summary>
        /// Nested 'Array.size' property.
        /// </summary>
        public SerializedProperty Size
        {
            get; private set;
        }

        /// <summary>
        /// Associated array property.
        /// </summary>
        public SerializedProperty List
        {
            get; private set;
        }


        /// <summary>
        /// Static representation of the standard list style.
        /// Provides all needed <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        internal static class Style
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float padding = 6.0f;

            internal static readonly float footerWidth = 60.0f;
            internal static readonly float footerButtonWidth = 25.0f;
            internal static readonly float footerButtonHeight = 13.0f;
            internal static readonly float footerMargin = 4.0f;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float footerPadding = 0.0f;
#else
            internal static readonly float footerPadding = 3.0f;
#endif
            internal static readonly float handleWidth = 15.0f;
            internal static readonly float handleHeight = 7.0f;
            internal static readonly float dragAreaWidth = 40.0f;
            internal static readonly float sizeAreaWidth = 19.0f;

            internal static readonly GUIContent sizePropertyContent;
            internal static readonly GUIContent iconToolbarAddContent;
            internal static readonly GUIContent iconToolbarDropContent;
            internal static readonly GUIContent iconToolbarRemoveContent;
            internal static readonly GUIContent emptyOrInvalidListContent;

            internal static readonly GUIStyle sizePropertyStyle;
            internal static readonly GUIStyle footerButtonStyle;
            internal static readonly GUIStyle dragHandleButtonStyle;
            internal static readonly GUIStyle headerBackgroundStyle;
            internal static readonly GUIStyle middleBackgroundStyle;
            internal static readonly GUIStyle footerBackgroundStyle;
            internal static readonly GUIStyle elementBackgroundStyle;

            static Style()
            {
                sizePropertyContent = EditorGUIUtility.TrTextContent("", "List size");
                iconToolbarAddContent = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
                iconToolbarDropContent = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
                iconToolbarRemoveContent = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                emptyOrInvalidListContent = EditorGUIUtility.TrTextContent("List is Empty");

                sizePropertyStyle = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleRight,
#if UNITY_2019_3_OR_NEWER
                    fixedHeight = 14.0f,
                    //in newer releases, the font size has to be adjusted
                    fontSize = 10
#endif
                };

                footerButtonStyle = new GUIStyle("RL FooterButton");
                dragHandleButtonStyle = new GUIStyle("RL DragHandle");
                headerBackgroundStyle = new GUIStyle("RL Header");
                middleBackgroundStyle = new GUIStyle("RL Background");
                footerBackgroundStyle = new GUIStyle("RL Footer");
                elementBackgroundStyle = new GUIStyle("RL Element");
            }
        }
    }
}