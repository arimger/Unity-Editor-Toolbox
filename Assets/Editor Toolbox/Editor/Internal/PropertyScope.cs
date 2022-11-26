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


        public PropertyScope(SerializedProperty property, GUIContent label)
        {
            this.property = property;
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


        public void Dispose()
        {
            ToolboxEditorGui.CloseProperty();
        }


        public bool IsVisible => property.isExpanded;
        public Rect LabelRect { get; private set; }
        public Rect InputRect { get; private set; }
    }
}