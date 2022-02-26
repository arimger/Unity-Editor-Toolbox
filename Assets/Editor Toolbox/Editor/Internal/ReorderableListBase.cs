using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Base abstract class for all reorderable list related implementations.
    /// Provides all needed properties and functions except property drawing.
    /// </summary>
    public abstract class ReorderableListBase
    {
        public delegate void DrawRectCallbackDelegate(Rect rect);

        public delegate bool CanChangeListCallbackDelegate(ReorderableListBase list);
        public delegate void IsChangedListCallbackDelegate(ReorderableListBase list);
        public delegate void ReorderedItemCallbackDelegate(ReorderableListBase list, int oldIndex, int newIndex);

        public delegate void DrawRelatedRectCallbackDelegate(Rect rect, ReorderableListBase list);
        public delegate void DrawIndexedRectCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

        public delegate object OverrideNewElementDelegate(int index);


        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawEmptyCallback;
        public DrawRectCallbackDelegate drawFooterCallback;

        public DrawRectCallbackDelegate drawHeaderBackgroundCallback;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback;
        public DrawRectCallbackDelegate drawMiddleBackgroundCallback;

        public DrawRelatedRectCallbackDelegate onAppendDropdownCallback;

        public IsChangedListCallbackDelegate onAppendCallback;
        public IsChangedListCallbackDelegate onRemoveCallback;
        public IsChangedListCallbackDelegate onSelectCallback;

        public IsChangedListCallbackDelegate onChangedCallback;
        public IsChangedListCallbackDelegate onMouseUpCallback;
        public ReorderedItemCallbackDelegate onReorderCallback;

        public CanChangeListCallbackDelegate onCanAppendCallback;
        public CanChangeListCallbackDelegate onCanRemoveCallback;

        public DrawIndexedRectCallbackDelegate drawElementCallback;
        public DrawIndexedRectCallbackDelegate drawElementHandleCallback;
        public DrawIndexedRectCallbackDelegate drawElementBackgroundCallback;

        public OverrideNewElementDelegate overrideNewElementCallback;


        protected const string defaultLabelFormat = "{0} {1}";
        protected const string defaultElementName = "Element";

        /// <summary>
        /// Hotcontrol index, unique for this instance.
        /// </summary>
        protected readonly int id = -1;

        protected float draggedY;


        public ReorderableListBase(SerializedProperty list)
            : this(list, null, true, true, false)
        { }

        public ReorderableListBase(SerializedProperty list, bool draggable)
            : this(list, null, draggable, true, false)
        { }

        public ReorderableListBase(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : this(list, elementLabel, draggable, hasHeader, fixedSize, true)
        { }

        public ReorderableListBase(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels)
            : this(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels, false)
        { }

        public ReorderableListBase(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels, bool foldable)
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
            FixedSize = fixedSize;
            HasLabels = hasLabels;
            ElementLabel = elementLabel;
            Foldable = foldable;

            //set serialized data
            List = list;
            Size = list.FindPropertyRelative("Array.size");

            Index = -1;
        }


        private void DoDraggingAndSelection()
        {
            var isClicked = false;
            var oldIndex = Index;
            var currentEvent = Event.current;

            switch (currentEvent.GetTypeForControl(id))
            {
                case EventType.KeyDown:
                    {
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

                    }

                    break;

                case EventType.MouseDown:
                    {
                        if (currentEvent.button != 0)
                        {
                            break;
                        }

                        //pick the active element based on mouse position
                        var selectedIndex = GetCoveredElementIndex(currentEvent.mousePosition);
                        if (selectedIndex == -1)
                        {
                            break;
                        }

                        Index = selectedIndex;

                        if (Draggable)
                        {
                            OnDrag(currentEvent);
                        }

                        //clicking on the list should end editing any fields
                        EditorGUIUtility.editingTextField = false;

                        SetKeyboardFocus();
                        currentEvent.Use();
                        isClicked = true;
                    }

                    break;

                case EventType.MouseDrag:
                    {
                        if (!Draggable || GUIUtility.hotControl != id)
                        {
                            break;
                        }

                        //set dragging state on first MouseDrag event after we got hotcontrol 
                        //to prevent animating elements when deleting elements by context menu
                        IsDragging = true;

                        Update(currentEvent);
                        currentEvent.Use();
                    }

                    break;

                case EventType.MouseUp:
                    {
                        if (!Draggable)
                        {
                            if (onMouseUpCallback != null && IsMouseInActiveElement())
                            {
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
                            var targetIndex = GetCoveredElementIndex(draggedY);
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
                                //give the user desired callbacks
                                onReorderCallback?.Invoke(this, oldActiveElement, newActiveElement);
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
                            OnDrop(currentEvent);
                        }
                    }

                    break;
            }

            //if the index has changed and there is a selected callback, call it
            if ((Index != oldIndex || isClicked))
            {
                onSelectCallback?.Invoke(this);
            }
        }


        protected abstract void DoListMiddle();

        protected abstract void DoListMiddle(Rect middleRect);


        protected virtual bool DoListHeader()
        {
            if (!HasHeader)
            {
                return false;
            }

            var rect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            return DoListHeader(rect);
        }

        protected virtual bool DoListHeader(Rect headerRect)
        {
            if (!HasHeader)
            {
                return false;
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

            HandleHeaderEvents(headerRect);
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

            return true;
        }

        protected virtual bool DoListFooter()
        {
            if (FixedSize)
            {
                return false;
            }

            var rect = GUILayoutUtility.GetRect(0, FooterHeight, GUILayout.ExpandWidth(true));
            return DoListFooter(rect);
        }

        protected virtual bool DoListFooter(Rect footerRect)
        {
            //ignore footer if list has fixed size
            if (FixedSize)
            {
                return false;
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

            return true;
        }

        protected virtual void OnDrag(Event currentEvent)
        {
            draggedY = GetDraggedY(currentEvent.mousePosition);
            GUIUtility.hotControl = id;
        }

        protected virtual void Update(Event currentEvent)
        {
            draggedY = GetDraggedY(currentEvent.mousePosition);
        }

        protected virtual void OnDrop(Event currentEvent)
        {
            GUIUtility.hotControl = 0;
        }

        protected virtual float GetDraggedY()
        {
            return GetDraggedY(Event.current.mousePosition);
        }

        protected virtual float GetDraggedY(Vector2 mousePosition)
        {
            return mousePosition.y;
        }

        protected virtual bool IsMouseInActiveElement()
        {
            //check if mouse position is inside current row rect 
            return GetCoveredElementIndex(Event.current.mousePosition.y) == Index;
        }

        protected virtual void HandleHeaderEvents(Rect rect)
        { }

        protected abstract int GetCoveredElementIndex(float localY);

        protected abstract int GetCoveredElementIndex(Vector2 mousePosition);


        public string GetElementDefaultName(int index)
        {
            return string.Format(defaultLabelFormat, defaultElementName, index);
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

        public GUIContent GetElementContent(SerializedProperty element, int index)
        {
            return HasLabels ? new GUIContent(GetElementDisplayName(element, index)) : GUIContent.none;
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

        public void AppendElement()
        {
            Index = (List.arraySize += 1) - 1;
            if (overrideNewElementCallback == null)
            {
                return;
            }

            //make sure serialized data is up-to-date
            List.serializedObject.ApplyModifiedProperties();
            var property = List.GetArrayElementAtIndex(Index);
            var newValue = overrideNewElementCallback(Index);
            //update property directly by the reflection
            property.SetProperValue(property.GetFieldInfo(), newValue, false);
        }

        public void RemoveElement()
        {
            RemoveElement(Index);
        }

        public void RemoveElement(int index)
        {
            List.DeleteArrayElementAtIndex(index);
            if (Index >= List.arraySize - 1)
            {
                Index = List.arraySize - 1;
            }
        }


        /// <summary>
        /// Draws whole list at once.
        /// </summary>
        public virtual void DoList()
        {
            //NOTE: indentation will break some controls
            //make sure there is no indent while drawing
            using (new ZeroIndentScope())
            {
                DoListHeader();
                DoListMiddle();
                DoListFooter();
            }

            DoDraggingAndSelection();
        }


        #region Methods: Default interaction/draw calls/controls

        /// <summary>
        /// Draws the default Footer.
        /// </summary>
        public virtual void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);

            //set rect properties from style
            var buttonWidth = Style.footerButtonWidth;
            var buttonHeight = Style.footerButtonHeight;
            var margin = Style.footerMargin;
            var padding = Style.footerPadding;

            //set proper rect for each button
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
        public virtual void DrawStandardFooterBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);
                Style.footerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        public virtual void DrawStandardName(Rect rect, GUIContent label, bool foldable)
        {
            if (foldable)
            {
                DrawStandardFoldout(rect, label);
            }
            else
            {
                DrawStandardLabel(rect, label);
            }
        }

        public virtual void DrawStandardLabel(Rect rect, GUIContent label)
        {
            EditorGUI.LabelField(rect, label, Style.namePropertyStyle);
        }

        public virtual void DrawStandardFoldout(Rect rect, GUIContent label)
        {
            var style = Style.foldoutLabelStyle;
            var leftPadding = style.padding.left;
            style.CalcMinMaxWidth(label, out var minWidth, out _);
            var diff = rect.height - style.CalcHeight(label, minWidth);
            rect.xMin += leftPadding;
            rect.xMax -= rect.width - minWidth;
            rect.yMin += diff / 2;
            rect.yMax -= diff / 2;
            List.isExpanded = EditorGUI.Foldout(rect, List.isExpanded, label, true, style);
        }

        /// <summary>
        /// Draws the default Header.
        /// </summary>
        public virtual void DrawStandardHeader(Rect rect)
        {
            var label = EditorGUI.BeginProperty(rect, TitleLabel, List);
            //display the property label using the preprocessed name
            DrawStandardName(rect, label, Foldable);
            using (new EditorGUI.DisabledScope(FixedSize))
            {
                var property = Size;
                var sizeValue = property.intValue;

                var potentialSizeContent = new GUIContent(sizeValue.ToString());
                var width = Style.sizePropertyStyle.CalcSize(potentialSizeContent).x;
                var diff = rect.height - Style.sizePropertyStyle.fixedHeight;
                rect.yMin += diff / 2;
                rect.yMax -= diff / 2;
                rect.xMin = rect.xMax - Mathf.Max(Style.sizeAreaWidth, width);

                EditorGUI.BeginProperty(rect, Style.sizePropertyContent, property);
                EditorGUI.BeginChangeCheck();
                //cache the size value using the delayed int field
                sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, sizeValue, Style.sizePropertyStyle), 0);
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
        public virtual void DrawStandardHeaderBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.headerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Element field.
        /// </summary>
        public virtual void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);
            //prepare dedicated label for target element
            var content = GetElementContent(element, index);
            EditorGUI.PropertyField(rect, element, content, element.isExpanded);
        }

        /// <summary>
        /// Draws the default dragging Handle.
        /// </summary>
        public virtual void DrawStandardElementHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
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
        public virtual void DrawStandardElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
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
                    var padding = Style.spacing + 1.0f;
#endif
                    rect.yMin -= padding / 2;
                    rect.yMax += padding / 2;
                }

                Style.elementBackgroundStyle.Draw(rect, false, selected, selected, focused);
            }
        }

        #endregion


        /// <summary>
        /// Index of the currently active (hovered) element.
        /// </summary>
        public int Index
        {
            get; set;
        }

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

        /// <summary>
        /// Indicates if list should allow dragging elements.
        /// </summary>
        public bool Draggable
        {
            get; set;
        }

        public bool IsDragging
        {
            get; protected set;
        }

        /// <summary>
        /// Indicates if list should be able to fold itself.
        /// </summary>
        public bool Foldable
        {
            get; set;
        }

        public bool IsExpanded
        {
            get => List.isExpanded || !Foldable;
        }

        public bool IsEmpty
        {
            get => Count == 0;
        }

        public bool IsPropertyValid
        {
            get => List != null && List.isArray;
        }

        /// <summary>
        /// Indicates if list should have add/remove controls.
        /// </summary>
        public bool FixedSize { get; set; }

        public bool HasHeader { get; set; } = true;

        public bool HasLabels { get; set; } = true;

#if UNITY_2020_1_OR_NEWER
        public virtual float HeaderHeight { get; set; } = 20.0f;
#else
        public virtual float HeaderHeight { get; set; } = 18.0f;
#endif
        public virtual float FooterHeight { get; set; } = 20.0f;

        /// <summary>
        /// Standard spacing between elements.
        /// </summary>
        public virtual float ElementSpacing { get; set; } = 5.0f;

        /// <summary>
        /// Current label of the List. NULL means it will be the default one.
        /// </summary>
        public GUIContent TitleLabel
        {
            get; set;
        }

        /// <summary>
        /// Custom element label name.
        /// "{<see cref="ElementLabel"/>} {index}"
        /// </summary>
        public string ElementLabel
        {
            get; set;
        }

        /// <summary>
        /// Child Array.size property.
        /// </summary>
        public SerializedProperty Size
        {
            get; protected set;
        }

        /// <summary>
        /// Associated array property.
        /// </summary>
        public SerializedProperty List
        {
            get; protected set;
        }

        /// <summary>
        /// Associated <see cref="SerializedObject"/>.
        /// </summary>
        public SerializedObject SerializedObject
        {
            get => List.serializedObject;
        }


        /// <summary>
        /// Static representation of the standard list styling.
        /// Provides all needed <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        protected static class Style
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float padding = 6.0f;

#if UNITY_2018_3_OR_NEWER
            internal static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
#else
            internal static readonly float lineHeight = 16.0f;
#endif
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
            internal static readonly float minEmptyHeight = 8.0f;

            internal static readonly Color selectionColor = new Color(0.3f, 0.47f, 0.75f);

            internal static readonly GUIContent sizePropertyContent;
            internal static readonly GUIContent iconToolbarAddContent;
            internal static readonly GUIContent iconToolbarDropContent;
            internal static readonly GUIContent iconToolbarRemoveContent;
            internal static readonly GUIContent emptyOrInvalidListContent;

            internal static readonly GUIStyle namePropertyStyle;
            internal static readonly GUIStyle foldoutLabelStyle;
            internal static readonly GUIStyle sizePropertyStyle;
            internal static readonly GUIStyle contentGroupStyle;
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

                namePropertyStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                foldoutLabelStyle = new GUIStyle(EditorStyles.foldout)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                sizePropertyStyle = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleRight,
                    //NOTE: the font size has to be adjusted in newer releases
#if UNITY_2019_3_OR_NEWER
                    fixedHeight = 14.0f,
                    fontSize = 10
#endif
                };
                contentGroupStyle = new GUIStyle(EditorStyles.inspectorFullWidthMargins);

                //built-in styles related to the ReorderableList controls
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