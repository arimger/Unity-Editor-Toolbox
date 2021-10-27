using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
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

        private void DrawEmptyList()
        {
            using (var emptyListGroup = new EditorGUILayout.VerticalScope(Style.contentGroupStyle))
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Style.minEmptyHeight));
                drawEmptyCallback?.Invoke(rect);
            }
        }

        private void DrawElementRow(int index, bool isActive, bool isTarget, bool hasFocus)
        {
            using (var elementRowGroup = new EditorGUILayout.HorizontalScope())
            {
                var elementRowRect = elementRowGroup.rect;
                var isSelected = isActive || isTarget;
                elementsRects[index] = elementRowRect;

                DrawElementBackground(elementRowRect, index, isSelected, hasFocus);
                DrawElementDragHandle(elementRowRect, index, isSelected, hasFocus);
                //draw the real property in separate vertical group
                using (var elementGroup = new EditorGUILayout.VerticalScope(Style.contentGroupStyle))
                {
                    var elementRect = elementGroup.rect;
                    //adjust label width to the known dragging area
                    EditorGUIUtility.labelWidth -= Style.dragAreaWidth;
                    DrawElement(index);
                    EditorGUIUtility.labelWidth += Style.dragAreaWidth;
                }

                //create additional space between item and right margin
                CreateSpace(Style.spacing);
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isSelected, bool hasFocus)
        {
            if (drawElementBackgroundCallback != null)
            {
                drawElementBackgroundCallback(rect, index, isSelected, hasFocus);
            }
            else
            {
                DrawStandardElementBackground(rect, index, isSelected, hasFocus, Draggable);
            }
        }

        private void DrawElementDragHandle(Rect rect, int index, bool isSelected, bool hasFocus)
        {
            DrawHandleArea();
            //draw handles only for static array or currently dragged element
            if (IsDragging && !isSelected)
            {
                return;
            }

            var handleRect = IsDragging
                ? GetHandleRect(draggedY)
                : GetHandleRect(rect);
            if (drawElementHandleCallback != null)
            {
                drawElementHandleCallback(handleRect, index, isSelected, hasFocus);
            }
            else
            {
                DrawStandardElementHandle(handleRect, index, isSelected, hasFocus, Draggable);
            }
        }

        private void DrawElement(int index)
        {
            var element = List.GetArrayElementAtIndex(index);
            var content = GetElementContent(element, index);
            ToolboxEditorGui.DrawToolboxProperty(element, content);
        }

        /// <summary>
        /// Preserves additional space for the dragging handle.
        /// </summary>
        private void DrawHandleArea()
        {
            CreateSpace(Style.dragAreaWidth);
        }

        /// <summary>
        /// Creates empty space in a layout-based structure.
        /// </summary>
        private void CreateSpace(float pixels)
        {
            GuiLayoutUtility.CreateSpace(pixels);
        }

        /// <summary>
        /// Creates empty space for the dragging area and returns adjusted <see cref="Rect"/>.
        /// </summary>
        private Rect GetHandleRect(Rect rowRect)
        {
            var handleRect = new Rect(rowRect);
            handleRect.xMax = handleRect.xMin + Style.dragAreaWidth;
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
        private void DrawTargetGap(int targetIndex, int draggingIndex)
        {
            var rect = elementsRects[targetIndex];
            rect.yMax = targetIndex < draggingIndex
                      ? rect.yMin
                      : rect.yMax + Style.spacing;
            rect.yMin = rect.yMax - Style.spacing;
            rect.xMin += Style.dragAreaWidth;
            EditorGUI.DrawRect(rect, Style.selectionColor);
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
                DrawEmptyList();
                return;
            }

            var upperPadding = Style.padding - ElementSpacing;
            var lowerPadding = Style.padding;
            CreateSpace(upperPadding);
            //if there are elements, we need to draw them - we will do
            //this differently depending on if we are dragging or not
            for (var i = 0; i < arraySize; i++)
            {
                //cache related properties
                var isActive = (i == Index);
                var hasFocus = (i == Index && HasKeyboardFocus());
                var isTarget = (i == lastCoveredIndex && !isActive);

                CreateSpace(ElementSpacing);
                DrawElementRow(i, isActive, isTarget, hasFocus);
                if (isTarget)
                {
                    DrawTargetGap(i, Index);
                }
            }

            CreateSpace(lowerPadding);
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

        protected override void HandleHeaderEvents(Rect rect)
        {
            base.HandleHeaderEvents(rect);
            DraggingUtility.DoDragAndDropForProperty(rect, List);
        }


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
    }
}