using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

//TODO:

namespace Toolbox.Editor.Internal
{
    public class ReorderableList2 : ReorderableListBase
    {
        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawVoidedCallback;
        public DrawRectCallbackDelegate drawFooterCallback;
        public DrawRectCallbackDelegate drawHandleCallback;

        public DrawRectCallbackDelegate drawHeaderBackgroundCallback;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback;


        private float draggedY;

        private int lastCoveredIndex = -1;

        private List<int> nonDragTargetIndices;

        private Rect[] elementsRects;


        public ReorderableList2(SerializedProperty list)
            : base(list)
        { }

        public ReorderableList2(SerializedProperty list, bool draggable)
            : base(list, draggable)
        { }

        public ReorderableList2(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : base(list, elementLabel, draggable, hasHeader, fixedSize)
        { }


        public void DoList()
        {
            using (new ZeroIndentScope())
            {
                DoListHeader();
                DoListMiddle();
                DoListFooter();
            }
        }


        private void DoListHeader()
        {
            if (!HasHeader)
            {
                return;
            }

            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(HeaderHeight));
            if (drawHeaderBackgroundCallback != null)
            {
                drawHeaderBackgroundCallback(rect);
            }
            else
            {
                DrawStandardHeaderBackground(rect);
            }

            rect.xMin += Style.padding / 2;
            rect.xMax -= Style.padding / 2;

            if (drawHeaderCallback != null)
            {
                drawHeaderCallback(rect);
            }
            else
            {
                DrawStandardHeader(rect);
            }
        }

        private void DoListMiddle()
        {
            GUILayout.Space(-Style.spacing);
            using (new EditorGUILayout.VerticalScope(Style.middleBackgroundStyle))
            {
                var arraySize = Count;
                if (List == null || List.isArray == false || arraySize == 0)
                {
                    var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Style.lineHeight));
                    if (drawVoidedCallback != null)
                    {
                        drawVoidedCallback(rect);
                    }
                    else
                    {
                        //TODO:
                    }
                }
                else
                {
                    ValidateElementsRects(arraySize);

                    GUILayout.Space(8.0f);
                    for (var i = 0; i < arraySize; i++)
                    {
                        var isActive = (i == Index);
                        var hasFocus = (i == Index && HasKeyboardFocus());
                        var isTarget = (i == lastCoveredIndex && !isActive);
                        var isEnding = (i == arraySize - 1);

                        //TODO:
                        using (var lineGroup = new EditorGUILayout.HorizontalScope())
                        {
                            //TODO:
                            if (Event.current.type == EventType.Repaint)
                            {
                                var backgroundRect = lineGroup.rect;
                                backgroundRect.xMin -= 4.0f;
                                backgroundRect.xMax += 4.0f;
                                var isSelected = isActive || isTarget;
                                DrawStandardElementBackground(backgroundRect, i, isSelected, hasFocus, Draggable);
                            }

                            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Style.lineHeight), GUILayout.Width(Style.dragAreaWidth));
                            if (!IsDragging)
                            {
                                if (drawHandleCallback != null)
                                {
                                    drawHandleCallback(rect);
                                }
                                else
                                {
                                    DrawStandardHandle(rect, i, isActive, hasFocus, Draggable);
                                }
                            }

                            using (var itemGroup = new EditorGUILayout.VerticalScope())
                            {
                                EditorGUIUtility.labelWidth -= Style.dragAreaWidth + 8.0f;
                                DrawStandardElement(i, isActive, hasFocus, Draggable);
                                EditorGUIUtility.labelWidth += Style.dragAreaWidth + 8.0f;
                            }
                        }

                        elementsRects[i] = GUILayoutUtility.GetLastRect();

                        //TODO: visualize dragging target
                        if (isTarget)
                        {
                            var elementsRect = elementsRects[i];
                            var draggingRect = new Rect(elementsRect);
                            if (i < Index)
                            {
                                draggingRect.yMin = elementsRect.yMin - 2.0f - 2.0f;
                                draggingRect.yMax = elementsRect.yMin - 2.0f;
                            }
                            else
                            {
                                draggingRect.yMin = elementsRect.yMax + 2.0f;
                                draggingRect.yMax = elementsRect.yMax + 2.0f + 2.0f;
                            }

                            draggingRect.xMin += Style.dragAreaWidth;
                            draggingRect.xMax += 2.0f;
                            EditorGUI.DrawRect(draggingRect, new Color(0.3f, 0.47f, 0.75f));

                            //if (Event.current.type == EventType.Repaint)
                            //{
                            //    var style = new GUIStyle("TV Insertion");
                            //    style.Draw(draggingRect, true, true, true, true);
                            //}
                        }

                        if (isEnding)
                        {
                            continue;
                        }

                        GUILayout.Space(ElementSpacing);
                    }

                    GUILayout.Space(8.0f);
                }
            }

            GUILayout.Space(-Style.spacing);

            if (IsDragging)
            {
                var fullRect = new Rect(elementsRects[0]);
                for (var i = 1; i < elementsRects.Length; i++)
                {
                    fullRect.yMax += elementsRects[i].height;
                }

                //TODO:
                var targetRect = new Rect(fullRect);
                targetRect.y = draggedY;
                targetRect.height = 0.0f;
                targetRect.yMin -= 10.0f;
                targetRect.yMax += 10.0f;
                targetRect.xMax = targetRect.xMin + EditorGUIUtility.labelWidth - 4.0f;

                var handleRect = new Rect(targetRect);
                handleRect.xMax = handleRect.xMin + Style.dragAreaWidth;
                targetRect.xMin = targetRect.xMin + Style.dragAreaWidth + 4.0f;

                if (drawHandleCallback != null)
                {
                    drawHandleCallback(handleRect);
                }
                else
                {
                    DrawStandardHandle(handleRect, Index, true, true, Draggable);
                }
            }

            DoDraggingAndSelection();
        }

        private void DoListFooter()
        {
            if (FixedSize)
            {
                return;
            }

            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(FooterHeight));
            if (drawFooterBackgroundCallback != null)
            {
                drawFooterBackgroundCallback(rect);
            }
            else
            {
                DrawStandardFooterBackground(rect);
            }

            if (drawFooterCallback != null)
            {
                drawFooterCallback(rect);
            }
            else
            {
                DrawStandardFooter(rect);
            }
        }


        private void DoDraggingAndSelection()
        {
            var currentEvent = Event.current;
            switch (currentEvent.GetTypeForControl(id))
            {
                case EventType.KeyDown:
                    {
                        if (GUIUtility.keyboardControl != id)
                        {
                            return;
                        }

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

                        Index = Mathf.Clamp(Index, 0, List.arraySize - 1);
                    }

                    break;

                case EventType.MouseDown:
                    {
                        if (currentEvent.button != 0)
                        {
                            break;
                        }

                        var selectedIndex = GetCoveredElementIndex(currentEvent.mousePosition.y);
                        if (selectedIndex == -1)
                        {
                            break;
                        }

                        Index = selectedIndex;

                        if (Draggable)
                        {
                            UpdateDraggedY(currentEvent.mousePosition);
                            GUIUtility.hotControl = id;
                            nonDragTargetIndices = new List<int>();
                        }

                        EditorGUIUtility.editingTextField = false;

                        SetKeyboardFocus();
                        currentEvent.Use();
                    }

                    break;

                case EventType.MouseDrag:
                    {
                        if (!Draggable || GUIUtility.hotControl != id)
                        {
                            break;
                        }

                        IsDragging = true;
                        lastCoveredIndex = GetCoveredElementIndex();

                        UpdateDraggedY();
                        currentEvent.Use();
                    }

                    break;

                case EventType.MouseUp:
                    {
                        if (!Draggable || GUIUtility.hotControl != id)
                        {
                            break;
                        }

                        currentEvent.Use();
                        IsDragging = false;

                        try
                        {
                            var targetIndex = GetCoveredElementIndex();
                            if (targetIndex != Index)
                            {
                                if (List != null)
                                {
                                    List.serializedObject.Update();
                                    List.MoveArrayElement(Index, targetIndex);
                                    List.serializedObject.ApplyModifiedProperties();
                                    GUI.changed = true;
                                }

                                var oldActiveElement = Index;
                                var newActiveElement = targetIndex;

                                Index = targetIndex;
                            }
                        }
                        finally
                        {
                            GUIUtility.hotControl = 0;
                            nonDragTargetIndices = null;
                            lastCoveredIndex = -1;
                        }
                    }

                    break;
            }
        }

        private void UpdateDraggedY()
        {
            UpdateDraggedY(Event.current.mousePosition);
        }

        private void UpdateDraggedY(Vector2 mousePosition)
        {
            if (elementsRects == null || elementsRects.Length == 0)
            {
                draggedY = mousePosition.y;
                return;
            }
            else
            {
                var rect1 = elementsRects[0];
                var rect2 = elementsRects.Last();
                draggedY = Mathf.Clamp(mousePosition.y, rect1.yMin, rect2.yMax);
            }
        }

        private void ValidateElementsRects(int arraySize)
        {
            if (elementsRects == null)
            {
                elementsRects = new Rect[arraySize];
                return;
            }

            if (elementsRects.Length != arraySize)
            {
                Array.Resize(ref elementsRects, arraySize);
                return;
            }
        }

        private int GetCoveredElementIndex()
        {
            return GetCoveredElementIndex(draggedY);
        }

        private int GetCoveredElementIndex(float draggedY)
        {
            if (elementsRects != null)
            {
                for (var i = 0; i < elementsRects.Length; i++)
                {
                    if (elementsRects[i].yMin <= draggedY &&
                        elementsRects[i].yMax >= draggedY)
                    {
                        return i;
                    }
                }
            }

            return -1;
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
            //EditorGUI.BeginDisabledGroup(onCanAppendCallback != null && !onCanAppendCallback(this));
            //if (GUI.Button(appendButtonRect, onAppendDropdownCallback != null
            //    ? Style.iconToolbarDropContent
            //    : Style.iconToolbarAddContent, Style.footerButtonStyle))
            if (GUI.Button(appendButtonRect, Style.iconToolbarAddContent, Style.footerButtonStyle))
            {
                //if (onAppendDropdownCallback != null)
                //{
                //    onAppendDropdownCallback(appendButtonRect, this);
                //}
                //else if (onAppendCallback != null)
                //{
                //    onAppendCallback(this);
                //}
                //else
                {
                    AppendElement();
                }

                //onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();

            var removeButtonRect = new Rect(rect.xMax - buttonWidth - margin, rect.y - padding, buttonWidth, buttonHeight);

            //EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            EditorGUI.BeginDisabledGroup(Index < 0 || Index >= Count);
            if (GUI.Button(removeButtonRect, Style.iconToolbarRemoveContent, Style.footerButtonStyle))
            {
                //if (onRemoveCallback != null)
                //{
                //    onRemoveCallback(this);
                //}
                //else
                {
                    RemoveElement();
                }

                //onChangedCallback?.Invoke(this);
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

            using (new EditorGUI.DisabledScope(FixedSize))
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
        public void DrawStandardElement(int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);
            var label = HasLabels 
                ? new GUIContent(GetElementDisplayName(element, index))
                : GUIContent.none;
            //TODO: label
            //if (Event.current.type == EventType.Layout)
            {
                ToolboxEditorGui.DrawToolboxProperty(element, label);
            }
        }

        /// <summary>
        /// Draws the default dragging Handle.
        /// </summary>
        public void DrawStandardHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
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

            internal static readonly float lineHeight = EditorGUIUtility.singleLineHeight;

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

            //TODO:
            //internal static readonly GUILayoutOption[] headerOptions;
            //internal static readonly GUILayoutOption[] voidedOptions;
            //internal static readonly GUILayoutOption[] footerOptions;
            //internal static readonly GUILayoutOption[] handleOptions;

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
                headerBackgroundStyle = new GUIStyle("RL Header")
                {
                    stretchHeight = false
                };
                middleBackgroundStyle = new GUIStyle("RL Background")
                {
                    stretchHeight = false
                };
                footerBackgroundStyle = new GUIStyle("RL Footer")
                {
                    stretchHeight = false
                };
                elementBackgroundStyle = new GUIStyle("RL Element")
                {
                    stretchHeight = false
                };
            }
        }
    }
}