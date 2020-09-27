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

        public delegate void DrawDetailedRectCallbackDelegate(Rect buttonRect, ReorderableList list);

        public delegate void ChangeListCallbackDelegate(ReorderableList list);

        public delegate bool CanChangeListCallbackDelegate(ReorderableList list);

        public delegate void ChangeDetailsCallbackDelegate(ReorderableList list, int oldIndex, int newIndex);

        public delegate bool CanChangeDetailsCallbackDelegate(ReorderableList list, int oldIndex, int newIndex);


        public DrawRectCallbackDelegate drawHeaderBackgroundCallback = null;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback = null;
        public DrawRectCallbackDelegate drawMiddleBackgroundCallback = null;

        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawVoidedCallback = null;
        public DrawRectCallbackDelegate drawFooterCallback = null;

        public DrawIndexedRectCallbackDelegate drawHandleCallback = null;
        public DrawIndexedRectCallbackDelegate drawElementCallback;
        public DrawIndexedRectCallbackDelegate drawElementBackgroundCallback;

        public DrawDetailedRectCallbackDelegate onAddDropdownCallback;


        public ChangeListCallbackDelegate onAddCallback;
        public ChangeListCallbackDelegate onSelectCallback;
        public ChangeListCallbackDelegate onChangedCallback;
        public ChangeListCallbackDelegate onRemoveCallback;
        public ChangeListCallbackDelegate onMouseUpCallback;
        public ChangeListCallbackDelegate onReorderCallback;

        public ChangeDetailsCallbackDelegate onDetailsCallback;

        public CanChangeListCallbackDelegate onCanAddCallback;
        public CanChangeListCallbackDelegate onCanRemoveCallback;


        public ElementCallbackDelegate elementHeightCallback;


        /// <summary>
        /// Hotcontrol index.
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
            id = GetHashCode();

            Draggable = draggable;
            HasHeader = hasHeader;
            HasFixedSize = hasFixedSize;
            ElementLabel = elementLabel;

            List = list;

            if (List != null && List.isArray == false)
            {
                throw new ArgumentException("Input elements should be an Array SerializedProperty.");
            }

            if (List != null && List.editable == false)
            {
                Draggable = false;
            }
        }


        /// <summary>
        /// Starts all drawing methods for each segment.
        /// </summary>
        /// <param name="headerRect"></param>
        /// <param name="midderRect"></param>
        /// <param name="footerRect"></param>
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
                    drawHeaderBackgroundCallback.Invoke(headerRect);
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
                    drawMiddleBackgroundCallback?.Invoke(middleRect);
                }
                else
                {
                    Style.middleBackground.Draw(middleRect, false, false, false, false);
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

            //handle empty list 
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
                    DrawStandardNoneElement(elementContentRect, Draggable);
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
                for (int i = 0; i < arraySize; i++)
                {
                    if (i != Index) nonDragTargetIndices.Add(i);
                }
                nonDragTargetIndices.Insert(targetIndex, -1);

                //now draw each element in the list (excluding the active element)
                var targetSeen = false;
                for (int i = 0; i < nonDragTargetIndices.Count; i++)
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
                    //draw the dragging handle
                    if (drawHandleCallback != null)
                    {
                        drawHandleCallback.Invoke(dragElementRect, i, false, false);
                    }
                    else
                    {
                        DrawStandardElementHandle(dragElementRect, i, false, false, Draggable);
                    }

                    elementContentRect = itemElementRect;
                    //adjust element's rect to the dragging handle
                    elementContentRect.xMin += Style.handleSpace;
                    elementContentRect.xMax -= Style.padding;

                    EditorGUIUtility.labelWidth -= Style.handleSpace;
                    //draw the actual element
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementContentRect, nonDragTargetIndices[i], false, false);
                    }
                    else
                    {
                        DrawStandardElement(elementContentRect, nonDragTargetIndices[i], false, false, Draggable);
                    }
                    EditorGUIUtility.labelWidth += Style.handleSpace;
                }

                //finally get the position of the active element
                elementY = draggedY - dragOffset + middleRect.y;
                itemElementRect.y = elementY;
                dragElementRect.y = elementY;
                //adjust rect height to desired element
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

                //draw the dragging handle
                if (drawHandleCallback != null)
                {
                    drawHandleCallback.Invoke(dragElementRect, Index, true, true);
                }
                else
                {
                    DrawStandardElementHandle(dragElementRect, Index, true, true, Draggable);
                }

                elementContentRect = itemElementRect;
                //adjust element's rect to the dragging handle
                elementContentRect.xMin += Style.handleSpace;
                elementContentRect.xMax -= Style.padding;

                EditorGUIUtility.labelWidth -= Style.handleSpace;
                //draw the active element
                if (drawElementCallback != null)
                {
                    drawElementCallback(elementContentRect, Index, true, true);
                }
                else
                {
                    DrawStandardElement(elementContentRect, Index, true, true, Draggable);
                }
                EditorGUIUtility.labelWidth += Style.handleSpace;
            }
            else
            {
                //if we aren't dragging, we just draw all of the elements in order
                for (int i = 0; i < arraySize; i++)
                {
                    var activeElement = (i == Index);
                    var focusedElement = (i == Index && HasKeyboardControl());

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
                        drawElementBackgroundCallback(itemElementRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElementBackground(itemElementRect, i, activeElement, focusedElement, Draggable);
                    }
                    //draw the dragging handle
                    if (drawHandleCallback != null)
                    {
                        drawHandleCallback.Invoke(dragElementRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElementHandle(dragElementRect, i, activeElement, focusedElement, Draggable);
                    }

                    elementContentRect = itemElementRect;
                    //adjust element's rect to the dragging handle
                    elementContentRect.xMin += Style.handleSpace;
                    elementContentRect.xMax -= Style.padding;

                    EditorGUIUtility.labelWidth -= Style.handleSpace;
                    //do the callback for the element
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementContentRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElement(elementContentRect, i, activeElement, focusedElement, Draggable);
                    }
                    EditorGUIUtility.labelWidth += Style.handleSpace;
                }
            }

            //handle the interaction
            DoDraggingAndSelection(middleRect);
        }

        private void DoListFooter(Rect footerRect)
        {
            //ignore footer if list has fixed size(no need for add/remove button)
            if (HasFixedSize) return;

            //draw the background on repaint
            if (Event.current.type == EventType.Repaint)
            {
                if (drawFooterBackgroundCallback != null)
                {
                    drawFooterBackgroundCallback.Invoke(footerRect);
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
            draggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, dragOffset, listRect.height - (GetElementHeight(Index) - dragOffset));
        }

        private void DoDraggingAndSelection(Rect listRect)
        {
            var clicked = false;
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
                    Index = GetRowIndex(currentEvent.mousePosition.y - listRect.y);

                    if (Draggable)
                    {
                        //if we can drag, set the hot control and start dragging (storing the offset)
                        dragOffset = (currentEvent.mousePosition.y - listRect.y) - GetElementYOffset(Index);
                        UpdateDraggedY(listRect);
                        GUIUtility.hotControl = id;
                        nonDragTargetIndices = new List<int>();
                    }

                    GrabKeyboardFocus();
                    currentEvent.Use();
                    clicked = true;
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
            if ((Index != oldIndex || clicked) && onSelectCallback != null)
            {
                onSelectCallback(this);
            }
        }

        private bool IsMouseInActiveElement(Rect listRect)
        {
            var mousePosition = Event.current.mousePosition;
            var mouseRowIndex = GetRowIndex(mousePosition.y - listRect.y);

            //check if mouse position is inside current row rect 
            return mouseRowIndex == Index && GetRowRect(mouseRowIndex, listRect).Contains(mousePosition);
        }

        private bool IsCurrentElementExpanded()
        {
            return List.GetArrayElementAtIndex(Index).isExpanded;
        }

        private int CalculateRowIndex()
        {
            return GetRowIndex(draggedY);
        }

        private int GetRowIndex(float localY)
        {
            var rowYOffset = 0.0f;
            for (int i = 0; i < Count; i++)
            {
                var rowYHeight = GetRowHeight(i);
                var rowYEnd = rowYOffset + rowYHeight;
                if (localY >= rowYOffset && localY < rowYEnd)
                {
                    return i;
                }
                rowYOffset += rowYHeight;
            }

            return Count - 1;
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

        private float GetListElementHeight()
        {
            var arraySize = Count;
            var listHeight = Style.padding * 2;
            if (arraySize != 0)
            {
                listHeight += GetElementYOffset(arraySize - 1) + GetRowHeight(arraySize - 1);
            }
            return listHeight;
        }

        private float GetElementYOffset(int index)
        {
            return GetElementYOffset(index, -1);
        }

        private float GetElementYOffset(int index, int skipIndex)
        {
            var offset = 0.0f;
            for (int i = 0; i < index; i++)
            {
                if (i != skipIndex) offset += GetRowHeight(i);
            }
            return offset;
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }


        public string GetElementName(SerializedProperty element, int index)
        {
            const string defaultPrefix = "Element";

            var elementLabel = element.displayName;
            if (ElementLabel == null)
            {
                return elementLabel;
            }

            var standardName = string.Format("{0} {1}", defaultPrefix, index);
            if (standardName != elementLabel)
            {
                return elementLabel;
            }

            return elementLabel.Replace(defaultPrefix, ElementLabel);
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public bool HasKeyboardControl()
        {
            return GUIUtility.keyboardControl == id;
        }

        public void RemoveKeyboardFocus()
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

        public void AddNewDefaultElement()
        {
            Index = (List.arraySize += 1) - 1;
        }

        public void RemoveSelectedElement()
        {
            List.DeleteArrayElementAtIndex(Index);
            if (Index >= List.arraySize - 1)
            {
                Index = List.arraySize - 1;
            }
        }

        /// <summary>
        /// Draws the default Footer.
        /// </summary>
        public void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.buttonSpace, rect.y, Style.buttonSpace, rect.height);

            //set rect properties from style
            var width = Style.buttonWidth;
            var height = Style.buttonHeight;
            var margin = Style.buttonMargin;
            var padding = Style.buttonPadding;

            //set proper rect for each buttons
            var addRect = new Rect(rect.xMin + margin, rect.y - padding, width, height);

            EditorGUI.BeginDisabledGroup(List.hasMultipleDifferentValues);
            EditorGUI.BeginDisabledGroup(onCanAddCallback != null && !onCanAddCallback(this));
            if (GUI.Button(addRect, onAddDropdownCallback != null ? Style.iconToolbarDrop : Style.iconToolbarAdd, Style.addRemoveButton))
            {
                if (onAddDropdownCallback != null)
                {
                    onAddDropdownCallback(addRect, this);
                }
                else if (onAddCallback != null)
                {
                    onAddCallback(this);
                }
                else
                {
                    AddNewDefaultElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();

            var removeRect = new Rect(rect.xMax - width - margin, rect.y - padding, width, height);

            EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            if (GUI.Button(removeRect, Style.iconToolbarRemove, Style.addRemoveButton))
            {
                if (onRemoveCallback != null)
                {
                    onRemoveCallback(this);
                }
                else
                {
                    RemoveSelectedElement();
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
                Style.footerBackground.Draw(new Rect(rect.xMax - Style.buttonSpace, rect.y, Style.buttonSpace, rect.height), false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Header.
        /// </summary>
        public void DrawStandardHeader(Rect rect)
        {
            var label = EditorGUI.BeginProperty(rect, new GUIContent(List.displayName), List);

            var diff = rect.height - Style.sizeLabel.fixedHeight;
            var oldY = rect.y;

#if !UNITY_2019_3_OR_NEWER
            //adjust OY position to middle of the conent
            rect.y += diff / 2;
#endif

            //display the property label using the preprocessed name by BeginProperty method
            EditorGUI.LabelField(rect, label);

            //adjust OY position to middle of the conent
            rect.y = oldY;
            rect.yMin += diff / 2;
            rect.yMax -= diff / 2;
            //adjust OX position and width for the size property
            rect.xMin = rect.xMax - Style.sizeArea;

            using (new EditorGUI.DisabledScope(HasFixedSize))
            {
                var property = Size;

                EditorGUI.BeginProperty(rect, Style.arraySizeFieldContent, property);
                EditorGUI.BeginChangeCheck();
                //cache a delayed size value using the delayed int field
                var sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, property.intValue, Style.sizeLabel), 0);
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
                Style.headerBackground.Draw(rect, false, false, false, false);
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
                    //NOTE: shadow appears before Unity 2019.3+
#if UNITY_2019_3_OR_NEWER
                    var padding = Style.spacing;
#else
                    var padding = Style.spacing + Style.spacing / 2;
#endif
                    rect.yMax += padding / 2;
                    rect.yMin -= padding / 2;
                }

                Style.elementBackground.Draw(rect, false, selected, selected, focused);
            }
        }

        /// <summary>
        /// Draws the default dragging Handle.
        /// </summary>
        public void DrawStandardElementHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                //keep the dragging handle in the 1 row
                rect.yMax = rect.yMin + Style.rowHeight;

                rect.width = Style.handleSpace;
                //prepare rect for the handle texture draw
                var xDiff = rect.width - Style.handleWidth;
                rect.xMin += xDiff / 2;
                rect.xMax = rect.xMin;
                rect.xMax += xDiff / 2;

                var yDiff = rect.height - Style.handleHeight;
                rect.yMin += yDiff / 2;
                rect.yMax -= yDiff / 2;
#if UNITY_2019_3_OR_NEWER
                rect.y += Style.spacing;
#endif
                //disable (if needed) and draw the handle
                using (var scope = new EditorGUI.DisabledScope(!draggable))
                {
                    Style.dragHandleButton.Draw(rect, false, false, false, false);
                }
            }
        }

        /// <summary>
        /// Draws the default Element field.
        /// </summary>
        public void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, new GUIContent(GetElementName(element, index)), element.isExpanded);
        }

        /// <summary>
        /// Draw the default none-Element.
        /// </summary>
        public void DrawStandardNoneElement(Rect rect, bool draggable)
        {
            EditorGUI.LabelField(rect, Style.listIsEmptyContent);
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

                //if we are handling multi-selection
                var smallerArraySize = List.arraySize;
                foreach (var targetObject in List.serializedObject.targetObjects)
                {
                    using (var serializedObject = new SerializedObject(targetObject))
                    {
                        var property = serializedObject.FindProperty(List.propertyPath);
                        smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
                    }
                }
                //return smalest array size
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
            get => GetListElementHeight();
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
        /// </summary>
        public string ElementLabel
        {
            get; set;
        } = "Element";

        /// <summary>
        /// Serialized 'Array.size' property.
        /// </summary>
        public SerializedProperty Size
        {
            get => List.FindPropertyRelative("Array.size");
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
            internal static readonly float sizeArea = 19.0f;
            internal static readonly float rowHeight = EditorGUIUtility.singleLineHeight;

            internal static readonly float buttonSpace = 60.0f;
            internal static readonly float buttonWidth = 25.0f;
            internal static readonly float buttonHeight = 13.0f;
            internal static readonly float buttonMargin = 4.0f;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float buttonPadding = 0.0f;
#else
            internal static readonly float buttonPadding = 3.0f;
#endif
            internal static readonly float handleSpace = 40.0f;
            internal static readonly float handleWidth = 15.0f;
            internal static readonly float handleHeight = 7.0f;

            internal static readonly GUIContent iconToolbarAdd;
            internal static readonly GUIContent iconToolbarDrop;
            internal static readonly GUIContent iconToolbarRemove;
            internal static readonly GUIContent listIsEmptyContent;
            internal static readonly GUIContent arraySizeFieldContent;

            internal static readonly GUIStyle sizeLabel;
            internal static readonly GUIStyle addRemoveButton;
            internal static readonly GUIStyle dragHandleButton;
            internal static readonly GUIStyle headerBackground;
            internal static readonly GUIStyle middleBackground;
            internal static readonly GUIStyle footerBackground;
            internal static readonly GUIStyle elementBackground;

            static Style()
            {
                iconToolbarAdd = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
                iconToolbarDrop = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
                iconToolbarRemove = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                listIsEmptyContent = EditorGUIUtility.TrTextContent("List is Empty");
                arraySizeFieldContent = EditorGUIUtility.TrTextContent("", "List size");

                sizeLabel = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleRight,
#if UNITY_2019_3_OR_NEWER
                    fixedHeight = 14.0f,
                    //in newer releases, the font size has to be adjusted
                    fontSize = 10
#endif
                };

                addRemoveButton = new GUIStyle("RL FooterButton");
                dragHandleButton = new GUIStyle("RL DragHandle");
                headerBackground = new GUIStyle("RL Header");
                middleBackground = new GUIStyle("RL Background");
                footerBackground = new GUIStyle("RL Footer");
                elementBackground = new GUIStyle("RL Element");
            }
        }
    }
}