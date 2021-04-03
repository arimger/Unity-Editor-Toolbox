using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Version of the <see cref="ReorderableList"/> dedicated for <see cref="ToolboxDrawer"/>s.
    /// Can be used only together with the internal layouting system.
    /// </summary>
    public class ToolboxList : ReorderableListBase
    {
        private int lastCoveredIndex = -1;

        private Rect[] elementsRects;


        public ToolboxList(SerializedProperty list)
            : base(list)
        { }

        public ToolboxList(SerializedProperty list, bool draggable)
            : base(list, draggable)
        { }

        public ToolboxList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : base(list, elementLabel, draggable, hasHeader, fixedSize)
        { }

        public ToolboxList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels)
            : base(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels)
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

        private Rect GetHandleRect()
        {
            return EditorGUILayout.GetControlRect(GUILayout.Height(Style.lineHeight), GUILayout.Width(Style.dragAreaWidth));
        }


        protected override void DoListMiddle()
        {
            using (var middleGroup = new EditorGUILayout.VerticalScope())
            {
                var middleRect = middleGroup.rect;
                DoListMiddle(middleRect);
            }

            //NOTE: we have to remove standard spacing because of created layout group
            GuiLayoutUtility.RemoveStandardSpacing();
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

                    using (var rowGroup = new EditorGUILayout.HorizontalScope())
                    {
                        var groupsRect = rowGroup.rect;
                        var isSelected = isActive || isTarget;
                        DrawStandardElementBackground(groupsRect, i, isSelected, hasFocus, Draggable);

                        var handleRect = GetHandleRect();
                        if (!IsDragging)
                        {
                            if (drawElementHandleCallback != null)
                            {
                                drawElementHandleCallback(handleRect, i, isActive, hasFocus);
                            }
                            else
                            {
                                DrawStandardElementHandle(handleRect, i, isActive, hasFocus, Draggable);
                            }
                        }

                        using (var elementGroup = new EditorGUILayout.VerticalScope())
                        {
                            var elementRect = elementGroup.rect;
                            //TODO: standard layout spacing
                            EditorGUIUtility.labelWidth -= Style.dragAreaWidth + 4.0f;
                            DrawStandardElement(elementRect, i, isActive, hasFocus, Draggable);
                            EditorGUIUtility.labelWidth += Style.dragAreaWidth + 4.0f;
                        }

                        //TODO: padding property
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

                if (IsDragging)
                {
                    var fullRect = new Rect(elementsRects[0]);
                    for (var i = 1; i < elementsRects.Length; i++)
                    {
                        fullRect.yMax += elementsRects[i].height;
                    }

                    var targetRect = new Rect(fullRect.x, draggedY, fullRect.width, 0.0f);
                    targetRect.yMin -= Style.lineHeight / 2;
                    targetRect.yMax += Style.lineHeight / 2;
                    targetRect.xMax = targetRect.xMin + Style.dragAreaWidth;

                    if (drawElementHandleCallback != null)
                    {
                        drawElementHandleCallback(targetRect, Index, true, true);
                    }
                    else
                    {
                        DrawStandardElementHandle(targetRect, Index, true, true, Draggable);
                    }
                }
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

        /// <inheritdoc/>
        public override void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
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