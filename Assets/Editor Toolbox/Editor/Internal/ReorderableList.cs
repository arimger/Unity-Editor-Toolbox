using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Custom implementation of <see cref="UnityEditorInternal.ReorderableList"/>.
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

            if (List != null && List.editable == false) Draggable = false;
            if (List != null && List.isArray == false)
            {
                throw new ArgumentException("Input elements should be an Array SerializedProperty.");
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
            var indentCounter = -EditorGUI.indentLevel;

            using (new EditorGUI.IndentLevelScope(indentCounter))
            {
                DoListHeader(headerRect);
                DoListMiddle(midderRect);
                DoListFooter(footerRect);
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
            var elementRect = middleRect;
            var dragElementRect = middleRect;
            //the content rect is what we will actually draw into -- it doesn't include the drag handle or padding
            var elementContentRect = elementRect;

            //handle empty list 
            if (List == null || List.isArray == false || arraySize == 0)
            {
                //there was no content, so we will draw an empty element
                elementRect.y = middleRect.y;
                //draw the background
                if (drawElementBackgroundCallback == null)
                {
                    DrawStandardElementBackground(elementRect, -1, false, false, false);
                }
                else
                {
                    drawElementBackgroundCallback(elementRect, -1, false, false);
                }

                elementContentRect = elementRect;
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

            //if there are elements, we need to draw them -- we will do this differently depending on if we are dragging or not
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
                    if (nonDragTargetIndices[i] != -1)
                    {
                        //update the height of the element
                        elementRect.height = GetElementHeight(i);
                        dragElementRect.height = GetElementHeight(i, false);

                        //update the position of the element
                        elementY = middleRect.y + GetElementYOffset(nonDragTargetIndices[i], Index);

                        if (targetSeen)
                        {
                            elementY += GetElementHeight(Index, true);
                        }

                        elementRect.y = elementY;
                        dragElementRect.y = elementY;

                        //draw the element background
                        if (drawElementBackgroundCallback != null)
                        {
                            drawElementBackgroundCallback(elementRect, i, false, false);
                        }
                        else
                        {
                            DrawStandardElementBackground(elementRect, i, false, false, Draggable);
                        }
                        //draw dragging handle
                        if (drawHandleCallback != null)
                        {
                            drawHandleCallback.Invoke(dragElementRect, i, false, false);
                        }
                        else
                        {
                            DrawStandardElementDraggingHandle(dragElementRect, i, false, false, Draggable);
                        }

                        elementContentRect = GetContentRect(elementRect);
                        //draw the actual element
                        if (drawElementCallback != null)
                        {
                            drawElementCallback(elementContentRect, nonDragTargetIndices[i], false, false);
                        }
                        else
                        {
                            DrawStandardElement(elementContentRect, List.GetArrayElementAtIndex(nonDragTargetIndices[i]), false, false, Draggable);
                        }
                    }
                    else
                    {
                        targetSeen = true;
                    }
                }

                //finally get the position of the active element
                elementY = draggedY - dragOffset + middleRect.y;
                elementRect.y = elementY;        
                dragElementRect.y = elementY;
                //adjust rect height to desired element
                elementRect.height = GetElementHeight(Index);

                //actually draw the element
                if (drawElementBackgroundCallback != null)
                {
                    drawElementBackgroundCallback(elementRect, Index, true, true);
                }
                else
                {
                    DrawStandardElementBackground(elementRect, Index, true, true, Draggable);
                }

                //draw dragging handle
                if (drawHandleCallback != null)
                {
                    drawHandleCallback.Invoke(dragElementRect, Index, true, true);
                }
                else
                {
                    DrawStandardElementDraggingHandle(dragElementRect, Index, true, true, Draggable);
                }

                elementContentRect = GetContentRect(elementRect);
                //draw the active element
                if (drawElementCallback != null)
                {
                    drawElementCallback(elementContentRect, Index, true, true);
                }
                else
                {
                    DrawStandardElement(elementContentRect, List.GetArrayElementAtIndex(Index), true, true, Draggable);
                }
            }
            else
            {
                //if we aren't dragging, we just draw all of the elements in order
                for (int i = 0; i < arraySize; i++)
                {
                    var activeElement = (i == Index);
                    var focusedElement = (i == Index && HasKeyboardControl());

                    //update the height of the element
                    elementRect.height = GetElementHeight(i);
                    dragElementRect.height = GetElementHeight(i, false);

                    //update the position of the element
                    elementY = middleRect.y + GetElementYOffset(i);
                    elementRect.y = elementY;
                    dragElementRect.y = elementY;

                    //draw the background
                    if (drawElementBackgroundCallback != null)
                    {
                        drawElementBackgroundCallback(elementRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElementBackground(elementRect, i, activeElement, focusedElement, Draggable);
                    }
                    //draw dragging handle
                    if (drawHandleCallback != null)
                    {
                        drawHandleCallback.Invoke(dragElementRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElementDraggingHandle(dragElementRect, i, activeElement, focusedElement, Draggable);
                    }

                    elementContentRect = GetContentRect(elementRect);
                    //do the callback for the element
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementContentRect, i, activeElement, focusedElement);
                    }
                    else
                    {
                        DrawStandardElement(elementContentRect, List.GetArrayElementAtIndex(i), activeElement, focusedElement, Draggable);
                    }
                }
            }

            //handle the interaction
            DoDraggingAndSelection(middleRect);
        }

        private void DoListHeader(Rect headerRect)
        {
            if (!HasHeader) return;

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
            headerRect.height -= Style.spacing;
            headerRect.xMin += Style.padding;
            headerRect.xMax -= Style.padding;
            headerRect.y += Style.spacing / 2;

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
                    if (GUIUtility.keyboardControl != id) return;
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
                        //TODO: do it cleaner;
                        //prevent expanded height
                        //List.GetArrayElementAtIndex(Index).isExpanded = false;
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
                        if (onMouseUpCallback != null && IsMouseInsideActiveElement(listRect))
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
                        if (Index != targetIndex)
                        {
                            //if the target index is different than the current index
                            if (List != null)
                            {
                                List.MoveArrayElement(Index, targetIndex);
                                List.serializedObject.ApplyModifiedProperties();
                                List.serializedObject.Update();
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

        private bool IsMouseInsideActiveElement(Rect listRect)
        {
            //cache current event
            var evt = Event.current;
            var mouseRowIndex = GetRowIndex(evt.mousePosition.y - listRect.y);
            //check if mouse position is inside current row rect 
            return mouseRowIndex == Index && GetRowRect(mouseRowIndex, listRect).Contains(evt.mousePosition);
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
            var listHeight = Style.padding + Style.padding;
            if (arraySize != 0)
            {
                listHeight += GetElementYOffset(arraySize - 1) + GetRowHeight(arraySize - 1);
            }
            return listHeight;
        }

        private Rect GetContentRect(Rect rect)
        {
            rect.xMax -= Style.padding;
            rect.xMin += Draggable 
                ? Style.handleArea 
                : Style.padding;
            return rect;
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }


        public void DoLayoutList()
        {
            var headerRect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            var middleRect = GUILayoutUtility.GetRect(0, MiddleHeight, GUILayout.ExpandWidth(true));
            var footerRect = GUILayoutUtility.GetRect(0, FooterHeight, GUILayout.ExpandWidth(true));
            DoList(headerRect, middleRect, footerRect);
        }

        public void DoNonLayoutList(Rect rect)
        {
            var headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            var middleRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, MiddleHeight);
            var footerRect = new Rect(rect.x, middleRect.y + middleRect.height, rect.width, FooterHeight);
            DoList(headerRect, middleRect, footerRect);
        }


        #region Default interaction/draw methods

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
        /// Default Footer behaviour.
        /// </summary>
        public void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.buttonArea, rect.y, Style.buttonArea, rect.height);
            //set rect properties from style
            var width = Style.buttonWidth;
            var height = Style.buttonHeight;
            var margin = Style.buttonMargin;
            var padding = Style.buttonPadding;
            //set proper rect for each buttons
            var addRect = new Rect(rect.xMin + margin, rect.y - padding, width, height);
            var removeRect = new Rect(rect.xMax - width - margin, rect.y - padding, width, height);

            EditorGUI.BeginDisabledGroup(List.hasMultipleDifferentValues);
            EditorGUI.BeginDisabledGroup(onCanAddCallback != null && !onCanAddCallback(this));
            if (GUI.Button(addRect, onAddDropdownCallback != null ? Style.iconToolbarDrop : Style.iconToolbarAdd, Style.footerButton))
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
            EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            if (GUI.Button(removeRect, Style.iconToolbarRemove, Style.footerButton))
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
        /// Draws Footer default background.
        /// </summary>
        public void DrawStandardFooterBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.footerBackground.Draw(new Rect(rect.xMax - Style.buttonArea, rect.y, Style.buttonArea, rect.height), false, false, false, false);
            }
        }

        /// <summary>
        /// Draws default Header's label.
        /// </summary>
        public void DrawStandardHeader(Rect rect)
        {
            //display property name
            EditorGUI.LabelField(rect, List.displayName);

            //adjust width and OX position for size property
            rect = new Rect(rect.xMax - Style.sizeArea, rect.y + (rect.height - Style.sizeLabel.fixedHeight) / 2, Style.sizeArea, rect.height);
 
            //display array size property without indentation
            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
            {
                var property = Size;

                EditorGUI.BeginDisabledGroup(HasFixedSize);
                EditorGUI.BeginProperty(rect, Style.arraySizeFieldContent, property);
                EditorGUI.BeginChangeCheck();
                //cache delayed size value using delayed int field
                var sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, property.intValue, Style.sizeLabel), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = sizeValue;
                }
                EditorGUI.EndProperty();
                EditorGUI.EndDisabledGroup();
            }
        }

        /// <summary>
        /// Draws default Header background.
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
        /// Draw default Element.
        /// </summary>
        public void DrawStandardElement(Rect rect, SerializedProperty element, bool selected, bool focused, bool draggable)
        {
            const string standardElementName = "Element";

            var displayName = element.displayName;          
            if (ElementLabel != null)
            {
                displayName = element.displayName.Replace(standardElementName, ElementLabel);
            }

            var displayContent = new GUIContent(displayName);
            EditorGUIUtility.labelWidth -= Style.handleArea;
            EditorGUI.PropertyField(rect, element, displayContent, element.isExpanded);
            EditorGUIUtility.labelWidth += Style.handleArea;
        }

        /// <summary>
        /// Draws default DraggingHandle.
        /// </summary>
        public void DrawStandardElementDraggingHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (draggable)
            {
                rect.height = Style.handleHeight;
                rect.width = Style.handleWidth;
                rect.y += (Style.handleHeight + rect.height) / 2 - Style.handleOffset;
                rect.x += Style.handleWidth / 2;
                Style.draggingHandle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws default Element background.
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
                    rect.height += padding;
                    rect.y -= padding / 2;
                }
                Style.elementBackground.Draw(rect, false, selected, selected, focused);
            }
        }

        /// <summary>
        /// Draw default Element.
        /// </summary>
        public void DrawStandardNoneElement(Rect rect, bool draggable)
        {
            EditorGUI.LabelField(rect, Style.listIsEmptyContent);
        }

        #endregion


        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == id) GUIUtility.keyboardControl = 0;
        }

        public bool HasKeyboardControl()
        {
            return GUIUtility.keyboardControl == id;
        }

        public float GetHeight()
        {
            return MiddleHeight + HeaderHeight + FooterHeight;
        }


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
                    var serializedObject = new SerializedObject(targetObject);
                    var property = serializedObject.FindProperty(List.propertyPath);
                    smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
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
        /// Serialized Array.size property.
        /// </summary>
        public SerializedProperty Size
        {
            get => List.FindPropertyRelative("Array.size");
        }

        /// <summary>
        /// Associated list property.
        /// </summary>
        public SerializedProperty List
        {
            get; private set;
        }


        /// <summary>
        /// Static representation of standard list style.
        /// Provides all needed <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        internal static class Style
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2;
#endif
            internal static readonly float padding = 6;
            internal static readonly float sizeArea = 19;

            internal static readonly float buttonArea = 60;
            internal static readonly float buttonWidth = 25;
            internal static readonly float buttonHeight = 13;
            internal static readonly float buttonMargin = 4;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float buttonPadding = 0;
#else
            internal static readonly float buttonPadding = 3;
#endif

            internal static readonly float handleArea = 40;
            internal static readonly float handleWidth = 15;
            internal static readonly float handleHeight = 7;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float handleOffset = 0;
#else
            internal static readonly float handleOffset = 2;
#endif

            internal static readonly GUIContent iconToolbarAdd;
            internal static readonly GUIContent iconToolbarDrop;
            internal static readonly GUIContent iconToolbarRemove;
            internal static readonly GUIContent listIsEmptyContent;
            internal static readonly GUIContent arraySizeFieldContent;

            internal static readonly GUIStyle sizeLabel;
            internal static readonly GUIStyle footerButton;
            internal static readonly GUIStyle draggingHandle;
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
                    alignment = TextAnchor.MiddleRight
                };

                footerButton = new GUIStyle("RL FooterButton");
                draggingHandle = new GUIStyle("RL DragHandle");
                headerBackground = new GUIStyle("RL Header");
                middleBackground = new GUIStyle("RL Background");
                footerBackground = new GUIStyle("RL Footer");
                elementBackground = new GUIStyle("RL Element");
            }
        }
    }
}