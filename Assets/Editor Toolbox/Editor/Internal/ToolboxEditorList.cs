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
    public class ToolboxEditorList : ReorderableListBase
    {
        private int lastCoveredIndex = -1;

        private Rect[] elementsRects;

        private Rect footerRect;
        private Rect middleRect;
        private Rect headerRect;


        public ToolboxEditorList(SerializedProperty list)
            : base(list)
        { }

        public ToolboxEditorList(SerializedProperty list, bool draggable)
            : base(list, draggable)
        { }

        public ToolboxEditorList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
            : base(list, elementLabel, draggable, hasHeader, fixedSize)
        { }

        public ToolboxEditorList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels)
            : base(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels)
        { }

        public ToolboxEditorList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize, bool hasLabels, bool foldable)
            : base(list, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable)
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

        private void DrawElementRow(int index, bool isActive, bool isTarget, bool hasFocus)
        {
            using (var elementRowGroup = new EditorGUILayout.HorizontalScope())
            {
                var elementRowRect = elementRowGroup.rect;
                var isSelected = isActive || isTarget;
                elementsRects[index] = elementRowRect;
                //draw element background (handle selection and targeting)
                if (drawElementBackgroundCallback != null)
                {
                    drawElementBackgroundCallback(elementRowRect, index, isSelected, hasFocus);
                }
                else
                {
                    DrawStandardElementBackground(elementRowRect, index, isSelected, hasFocus, Draggable);
                }

                //prepare handle-related properties
                var handleRect = GetHandleRect(elementRowRect);
                var drawHandle = !IsDragging;
                //draw handles only for static array or currently dragged element
                if (isSelected && IsDragging)
                {
                    drawHandle = true;
                    handleRect = GetHandleRect(draggedY);
                }

                //draw dragging handle
                if (drawHandle)
                {
                    if (drawElementHandleCallback != null)
                    {
                        drawElementHandleCallback(handleRect, index, isActive, hasFocus);
                    }
                    else
                    {
                        DrawStandardElementHandle(handleRect, index, isActive, hasFocus, Draggable);
                    }
                }

                //draw the real property in separate vertical group
                using (var elementGroup = new EditorGUILayout.VerticalScope(Style.contentGroupStyle))
                {
                    var elementRect = elementGroup.rect;
                    //adjust label width to the known dragging area
                    EditorGUIUtility.labelWidth -= Style.dragAreaWidth;
                    if (drawElementCallback != null)
                    {
                        drawElementCallback(elementRect, index, isActive, hasFocus);
                    }
                    else
                    {
                        DrawStandardElement(elementRect, index, isActive, hasFocus, Draggable);
                    }
                    EditorGUIUtility.labelWidth += Style.dragAreaWidth;
                }

                //create additional space between element and right margin
                DrawEmptySpace(Style.spacing);
            }
        }

        private void DrawVacantList()
        {
            using (var emptyListGroup = new EditorGUILayout.VerticalScope(Style.contentGroupStyle))
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Style.minEmptyHeight));
                if (drawVacantCallback != null)
                {
                    drawVacantCallback(rect);
                }
                else
                {
                    //TODO: how about additional label?
                }
            }
        }

        /// <summary>
        /// Creates empty space in a layout-based structure.
        /// </summary>
        private void DrawEmptySpace(float space)
        {
            GuiLayoutUtility.CreateSpace(space);
        }

        /// <summary>
        /// Creates empty space for the dragging area and returns adjusted <see cref="Rect"/>.
        /// </summary>
        private Rect GetHandleRect(Rect rowRect)
        {
            var handleRect = new Rect(rowRect);
            handleRect.xMax = handleRect.xMin + Style.dragAreaWidth;
            DrawEmptySpace(Style.dragAreaWidth);
            return handleRect;
        }

        /// <summary>
        /// Creates <see cref="Rect"/> to represent the position of the dragged handle.
        /// It's based on <see cref="elementsRects"/> so it's crucial to update known rects before a call.
        /// </summary>
        private Rect GetHandleRect(float draggedY)
        {
            var fullRect = new Rect(elementsRects[0]);
            for (var i = 1; i < elementsRects.Length; i++)
            {
                fullRect.yMax += elementsRects[i].height;
            }

            var rect = new Rect(fullRect.x, draggedY, fullRect.width, 0.0f);
            rect.yMin -= Style.lineHeight / 2;
            rect.yMax += Style.lineHeight / 2;
            rect.xMax = rect.xMin + Style.dragAreaWidth;
            return rect;
        }

        /// <summary>
        /// Creates small, colored rect to visualize the dragging target (gap between elements).
        /// </summary>
        private Rect DrawTargetGap(int targetIndex, int draggingIndex, Color color, float width, float margin)
        {
            //TODO: refactor + custom texture/method for the gap
            var targetsRect = elementsRects[targetIndex];
            var rect = new Rect(targetsRect);
            //handle differently situation above and under target
            if (targetIndex < draggingIndex)
            {
                rect.yMax = rect.yMin;
                rect.yMin = rect.yMax - width;
            }
            else
            {
                rect.yMin = rect.yMax;
                rect.yMax = rect.yMin + width;
            }

            rect.xMin += margin;
            EditorGUI.DrawRect(rect, color);
            return rect;
        }


        protected override void DoListMiddle()
        {
            using (var middleGroup = new EditorGUILayout.VerticalScope())
            {
                var middleRect = middleGroup.rect;
                DoListMiddle(middleRect);
            }
        }

        protected override void DoListMiddle(Rect middleRect)
        {
            this.middleRect = middleRect;
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

            var arraySize = Count;
            //make sure rects array is valid
            ValidateElementsRects(arraySize);
            //handle empty or invalid array 
            if (!IsPropertyValid || !IsExpanded || IsEmpty)
            {
                DrawVacantList();
                return;
            }

            DrawEmptySpace(Style.padding);
            //if there are elements, we need to draw them - we will do
            //this differently depending on if we are dragging or not
            for (var i = 0; i < arraySize; i++)
            {
                //cache related properties
                var isActive = (i == Index);
                var hasFocus = (i == Index && HasKeyboardFocus());
                var isTarget = (i == lastCoveredIndex && !isActive);
                var isEnding = (i == arraySize - 1);

                //draw current array element
                DrawElementRow(i, isActive, isTarget, hasFocus);

                //draw dragging element gap
                if (isTarget)
                {
                    DrawTargetGap(i, Index, GapColor, GapWidth, Style.dragAreaWidth);
                }

                if (isEnding)
                {
                    continue;
                }

                //create spacing for elements
                DrawEmptySpace(ElementSpacing);
            }

            DrawEmptySpace(Style.padding);
        }

        protected override bool DoListHeader()
        {
            return base.DoListHeader();
        }

        protected override bool DoListHeader(Rect headerRect)
        {
            this.headerRect = headerRect;
            return base.DoListHeader(headerRect);
        }

        protected override bool DoListFooter()
        {
            return base.DoListFooter();
        }

        protected override bool DoListFooter(Rect footerRect)
        {
            this.footerRect = footerRect;
            return base.DoListFooter(footerRect);
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

        protected override int GetCoveredElementIndex(Vector2 mousePosition)
        {
            return middleRect.Contains(mousePosition) ? GetCoveredElementIndex(mousePosition.y) : -1;
        }


        #region Methods: Default interaction/draw calls

        /// <inheritdoc/>
        public override void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);
            var content = GetElementContent(element, index);
            ToolboxEditorGui.DrawToolboxProperty(element, content);
        }

        #endregion

        /// <inheritdoc/>
        public override void DoList()
        {
            //pack eveything in one, vertical scope
            //it will keep sections always in order
            using (new EditorGUILayout.VerticalScope())
            {
                base.DoList();
            }
        }


        /// <inheritdoc/>
        public override float ElementSpacing { get; set; } = 1.0f;

        public Rect LastHeaderRect
        {
            get => headerRect;
        }

        public Rect LastMiddleRect
        {
            get => middleRect;
        }

        public Rect LastFooterRect
        {
            get => footerRect;
        }

        public Color GapColor { get; set; } = new Color(0.3f, 0.47f, 0.75f);

        public float GapWidth { get; set; } = 2.0f;
    }
}