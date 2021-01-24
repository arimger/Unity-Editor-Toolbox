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


        protected override void DoListMiddle()
        {
            //GUILayout.Space(-Style.spacing);
            using (var middleGroup = new EditorGUILayout.VerticalScope())
            {
                DoListMiddle(middleGroup.rect);
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
        }

        protected override void DoListMiddle(Rect middleRect)
        {
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
                            EditorGUIUtility.labelWidth -= Style.dragAreaWidth + 4.0f;
                            DrawStandardElement(i, isActive, hasFocus, Draggable);
                            EditorGUIUtility.labelWidth += Style.dragAreaWidth + 4.0f;
                        }

                        GUILayout.Space(4.0f);
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
                        //draggingRect.xMax += 2.0f;
                        EditorGUI.DrawRect(draggingRect, new Color(0.3f, 0.47f, 0.75f));
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


        public override void DoList()
        {
            base.DoList();

            DoDraggingAndSelection();
        }

        #region Methods: Default interaction/draw calls

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

        #endregion
    }
}