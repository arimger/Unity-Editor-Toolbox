using System;
using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor
{
    public class ToolboxEditorDrawer : IToolboxEditorDrawer
    {
        private readonly HashSet<string> propertiesToIgnore = new HashSet<string>();
        //TODO:
        private readonly Action<SerializedProperty> toolboxDrawingAction;
        private readonly Action<SerializedProperty> defaultDrawingAction;

        public ToolboxEditorDrawer()
        {
            toolboxDrawingAction = ToolboxEditorGui.DrawToolboxProperty;
            defaultDrawingAction = ToolboxEditorGui.DrawNativeProperty;
        }

        private void DrawProperty(SerializedProperty property, Action<SerializedProperty> drawingAction)
        {
            var propertyPath = property.propertyPath;
            if (IsPropertyIgnored(propertyPath))
            {
                return;
            }

            drawingAction(property);
        }

        /// <inheritdoc />
        public void DrawEditor(SerializedObject serializedObject)
        {
            if (ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                DrawToolboxEditor(serializedObject);
            }
            else
            {
                DrawDefaultEditor(serializedObject);
            }
        }

        /// <inheritdoc />
        public void DrawEditor(SerializedObject serializedObject, Action<SerializedProperty> drawingAction)
        {
            serializedObject.UpdateIfRequiredOrScript();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                var isScript = PropertyUtility.IsDefaultScriptProperty(property);
                using (new EditorGUI.DisabledScope(isScript))
                {
                    DrawProperty(property.Copy(), drawingAction);
                }

                while (property.NextVisible(false))
                {
                    DrawProperty(property.Copy(), drawingAction);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <inheritdoc />
        public void DrawToolboxEditor(SerializedObject serializedObject)
        {
            DrawEditor(serializedObject, ToolboxEditorGui.DrawToolboxProperty);
        }

        /// <inheritdoc />
        public void DrawDefaultEditor(SerializedObject serializedObject)
        {
            DrawEditor(serializedObject, ToolboxEditorGui.DrawNativeProperty);
        }

        /// <inheritdoc />
        public void IgnoreProperty(SerializedProperty property)
        {
            IgnoreProperty(property.propertyPath);
        }

        /// <inheritdoc />
        public void IgnoreProperty(string propertyPath)
        {
            propertiesToIgnore.Add(propertyPath);
        }

        /// <inheritdoc />
        public bool IsPropertyIgnored(string propertyPath)
        {
            return propertiesToIgnore.Contains(propertyPath);
        }
    }
}