using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Mirrors functionality of the <see cref="EditorGUI.PropertyScope"/> for layout-based controls.
    /// Additionally creates foldout label for the given property.
    /// </summary>
    internal class PropertyScope : IDisposable
    {
        private readonly SerializedProperty property;

        public PropertyScope(SerializedProperty property, GUIContent label, Action<GUIContent> drawLabelAction = null)
        {
            this.property = property;
            using (var labelScope = new EditorGUILayout.VerticalScope())
            {
                label = EditorGUI.BeginProperty(labelScope.rect, label, property);
                if (drawLabelAction != null)
                {
                    drawLabelAction(label);
                }
                else
                {
                    property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, true);
                }
            }
        }

        public void Dispose()
        {
            EditorGUI.EndProperty();
        }

        public bool IsVisible => property.isExpanded;
    }
}
