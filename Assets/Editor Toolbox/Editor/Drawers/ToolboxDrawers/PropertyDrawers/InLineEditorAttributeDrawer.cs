using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    using Editor = UnityEditor.Editor;

    public class InLineEditorAttributeDrawer : ToolboxSelfPropertyDrawer<InLineEditorAttribute>
    {
        /// <summary>
        /// Collection of all stored <see cref="Editor"/> instances.
        /// </summary>
        private static Dictionary<string, Editor> editorInstances = new Dictionary<string, Editor>();


        /// <summary>
        /// Clears and destroys particular editor mapped to the provided key.
        /// </summary>
        /// <param name="key"></param>
        private void ClearEditor(string key)
        {
            if (editorInstances.TryGetValue(key, out var editor))
            {
                editorInstances.Remove(key);
                Object.DestroyImmediate(editor);
            }
        }

        /// <summary>
        /// Clears and destroys all previously instantiated editor instances.
        /// </summary>
        private void ClearEditors()
        {
            foreach (var editor in editorInstances.Values)
            {
                Object.DestroyImmediate(editor);
            }

            editorInstances.Clear();
        }

        /// <summary>
        /// Draws the inlined version of the <see cref="Editor"></see> and handles all unexpected situations.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void OnEditorGuiSafe(Editor editor, InLineEditorAttribute attribute)
        {
            //force the expanded inspector since a header is not expected
            InspectorUtility.SetIsEditorExpanded(editor, true);

            var labelWidth = 0.0f;
            var fieldWidth = 0.0f;

            //prevent custom editors for overriding the internal properties
            labelWidth = EditorGUIUtility.labelWidth;
            fieldWidth = EditorGUIUtility.fieldWidth;

            OnEditorGuiDraw(editor, attribute);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }

        /// <summary>
        /// Draws the inlined version of the <see cref="Editor"></see>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void OnEditorGuiDraw(Editor editor, InLineEditorAttribute attribute)
        {
            //begin editor inside vertical group
            EditorGUILayout.BeginVertical(Style.inlinedStyle);
            EditorGUILayout.BeginVertical();

            //draw whole inspector and apply all changes 
            editor.serializedObject.Update();
            editor.OnInspectorGUI();
            editor.serializedObject.ApplyModifiedProperties();

            if (editor.HasPreviewGUI())
            {
                //draw preview if possible and needed
                if (attribute.DrawPreview)
                {
                    editor.OnPreviewGUI(EditorGUILayout.GetControlRect(false, attribute.PreviewHeight), Style.previewStyle);
                }

                if (attribute.DrawSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    editor.OnPreviewSettings();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Handles the property drawing process and tries to create a inlined version of the <see cref="Editor"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, InLineEditorAttribute attribute)
        {
            var propertyKey = property.GetPropertyKey();

            //create a standard property field
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
            if (EditorGUI.EndChangeCheck())
            {
                ClearEditor(propertyKey);
                return;
            }

            //NOTE: multiple different Editors are not supported
            if (property.hasMultipleDifferentValues)
            {
                return;
            }

            var propertyValue = property.objectReferenceValue;
            if (propertyValue == null)
            {
                return;
            }

            //get (or create) editor for the current property
            if (!editorInstances.TryGetValue(propertyKey, out var editor))
            {
                editor = Editor.CreateEditor(propertyValue);
                if (editor.HasPreviewGUI())
                {
                    editor.ReloadPreviewInstances();
                }
                editorInstances[propertyKey] = editor;
            }

            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, "Inspector Preview", true, Style.foldoutStyle))
            {
                //draw and prewarm the inlined editor   
                OnEditorGuiSafe(editor, attribute);
            }
        }


        /// <summary>
        /// Handles data clearing between editors.
        /// </summary>
        public override void OnGuiReload()
        {
            ClearEditors();
        }

        /// <summary>
        /// Checks if the provided property can be handled by this drawer.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private static class Style
        {
            internal static readonly GUIStyle inlinedStyle;
            internal static readonly GUIStyle foldoutStyle;
            internal static readonly GUIStyle previewStyle;

            static Style()
            {
                inlinedStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(13, 12, 8, 8)
                };
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontSize = 9,
                    alignment = TextAnchor.MiddleLeft
                };

                previewStyle = new GUIStyle();
                previewStyle.normal.background = EditorGuiUtility.CreatePersistantTexture();
            }
        }
    }
}