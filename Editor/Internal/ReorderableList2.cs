using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Experimental version of the <see cref="ReorderableList"/> dedicated for <see cref="ToolboxDrawer"/>s.
    /// </summary>
    public class ReorderableList2 : ReorderableListBase
    {
        public delegate void DrawElementCallbackDelegate(int index, bool isActive, bool isFocused);

        public DrawRectCallbackDelegate drawHandleCallback;
        //TODO:
        public DrawElementCallbackDelegate drawElementCallback;


        private int lastCoveredIndex = -1;

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


        protected override void DoListMiddle()
        {
            using (var middleGroup = new EditorGUILayout.VerticalScope())
            {
                DoListMiddle(middleGroup.rect);
            }

            //NOTE: we have to remove standard spacing because of created layout group
            GuiLayoutUtility.RemoveStandardSpacing();

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

                if (drawHandleCallback != null)
                {
                    drawHandleCallback(handleRect);
                }
                else
                {
                    DrawStandardElementHandle(handleRect, Index, true, true, Draggable);
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
                                DrawStandardElementHandle(rect, i, isActive, hasFocus, Draggable);
                            }
                        }

                        using (var itemGroup = new EditorGUILayout.VerticalScope())
                        {
                            EditorGUIUtility.labelWidth -= Style.dragAreaWidth + 4.0f;
                            DrawStandardElement(i, isActive, hasFocus, Draggable);
                            EditorGUIUtility.labelWidth += Style.dragAreaWidth + 4.0f;
                        }

                        GUILayout.Space(6.0f);
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

        protected override void OnDrag(Event currentEvent)
        {
            base.OnDrag(currentEvent);
            lastCoveredIndex = Index;
        }

        protected override void Update(Event currentEvent)
        {
            base.Update(currentEvent);
            lastCoveredIndex = GetCoveredElementIndex(draggedY);
        }

        protected override void OnDrop(Event currentEvent)
        {
            base.OnDrop(currentEvent);
            lastCoveredIndex = -1;
        }

        protected override float GetDraggedY(Vector2 mousePosition)
        {
            if (elementsRects == null || elementsRects.Length == 0)
            {
                return mousePosition.y;
            }
            else
            {
                var spacing = ElementSpacing;
                var minRect = elementsRects.First();
                var maxRect = elementsRects.Last();
                return Mathf.Clamp(mousePosition.y, minRect.yMin - spacing, maxRect.yMax + spacing);
            }
        }

        protected override int GetCoveredElementIndex(float localY)
        {
            if (elementsRects != null)
            {
                for (var i = 0; i < elementsRects.Length; i++)
                {
                    if (elementsRects[i].yMin <= localY &&
                        elementsRects[i].yMax >= localY)
                    {
                        return i;
                    }
                }
            }

            return -1;
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
                : new GUIContent();
            ToolboxEditorGui.DrawToolboxProperty(element, label);
        }

        #endregion
    }
}