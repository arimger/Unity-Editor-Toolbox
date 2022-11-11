using System;
using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor
{
    public class ToolboxEditorDrawer : IToolboxEditorDrawer
    {
        private readonly HashSet<string> propertiesToIgnore = new HashSet<string>();

        //TODO: refactor
        /// <inheritdoc />
        public void DrawToolboxProperty(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            if (propertiesToIgnore.Contains(propertyPath))
            {
                return;
            }

            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        /// <inheritdoc />
        public void DrawDefaultProperty(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            if (propertiesToIgnore.Contains(propertyPath))
            {
                return;
            }

            ToolboxEditorGui.DrawNativeProperty(property);
        }

        /// <inheritdoc />
        public void DrawToolboxInspector(SerializedObject serializedObject)
        {
            if (!ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector(serializedObject);
                return;
            }

            //TODO: how to handle default/toolbox drawers
            //Action<SerializedProperty> drawAction = ToolboxDrawerModule.ToolboxDrawersAllowed
            //    ? (Action<SerializedProperty>)DrawToolboxProperty
            //    : DrawDefaultProperty;
            serializedObject.UpdateIfRequiredOrScript();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                var isScript = PropertyUtility.IsDefaultScriptProperty(property);
                using (new EditorGUI.DisabledScope(isScript))
                {
                    DrawToolboxProperty(property.Copy());
                }

                while (property.NextVisible(false))
                {
                    DrawToolboxProperty(property.Copy());
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        //TODO:
        /// <inheritdoc />
        public void DrawDefaultInspector(SerializedObject serializedObject)
        {
            serializedObject.UpdateIfRequiredOrScript();
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                var isScript = PropertyUtility.IsDefaultScriptProperty(property);
                using (new EditorGUI.DisabledScope(isScript))
                {
                    EditorGUILayout.PropertyField(property, true);
                }

                while (property.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
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
    }
}
