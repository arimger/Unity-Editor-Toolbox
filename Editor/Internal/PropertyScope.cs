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
            var rowHeight = EditorGUIUtility.singleLineHeight;
            var labelRect = EditorGUILayout.GetControlRect(true, rowHeight);
            label = EditorGUI.BeginProperty(labelRect, label, property);
            HandleEvents(labelRect);
            TryDrawLabel(labelRect, label);
        }


        private void HandleEvents(Rect rect)
        {
            if (property.isArray)
            {
                DragAndDropUtility.DoDragAndDropForProperty(rect, property);
            }
        }

        private void TryDrawLabel(Rect rect, GUIContent label)
        {
            if (property.hasChildren)
            {
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.LabelField(rect, label);
            }
        }


        public void Dispose()
        {
            EditorGUI.EndProperty();
        }


        public bool IsVisible => property.isExpanded;
    }
}
