using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Mirrors functionality of the <see cref="EditorGUI.PropertyScope"/> for layout-based controls.
    /// Additionally creates foldout label for the given property.
    /// </summary>
    internal class PropertyScope : IDisposable
    {
        private readonly SerializedProperty property;
        private readonly bool closeManually;
        private bool isClosed;

        public PropertyScope(SerializedProperty property, GUIContent label) : this(property, label, false)
        { }

        public PropertyScope(SerializedProperty property, GUIContent label, bool closeManually)
        {
            this.property = property;
            this.closeManually = closeManually;
            isClosed = false;

            ToolboxEditorGui.BeginProperty(property, ref label, out var rect);
            HandleEvents(rect);
            TryDrawLabel(rect, label);
        }

        private void HandleEvents(Rect rect)
        {
            if (property.isArray)
            {
                DraggingUtility.DoDragAndDropForProperty(rect, property);
            }
        }

        private void TryDrawLabel(Rect rect, GUIContent label)
        {
            InputRect = rect;
            var size = EditorStyles.label.CalcSize(label);
            rect.xMax = rect.xMin + size.x;
            rect.xMax += EditorGuiUtility.IndentSize;
            rect.xMax += EditorGuiUtility.SpacingSize;
            LabelRect = rect;
            if (property.hasVisibleChildren)
            {
                property.isExpanded = EditorGUI.Foldout(LabelRect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.LabelField(LabelRect, label);
            }
        }

        public void Close()
        {
            ToolboxEditorGui.CloseProperty();
            isClosed = true;
        }

        public void Dispose()
        {
            if (closeManually || isClosed)
            {
                return;
            }

            Close();
        }

        /// <summary>
        /// Indicates whether property is expanded and has any children to draw.
        /// </summary>
        public bool IsVisible => property.isExpanded && property.hasVisibleChildren;
        public Rect LabelRect { get; private set; }
        public Rect InputRect { get; private set; }
    }
}