using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

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
        /// Clears and destroys particular Editor mapped to the provided key.
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
        /// Clears and destroys all previously instantiated Editor instances.
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
        /// Draws the inlined version of the given <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void DrawEditor(Editor editor, InLineEditorAttribute attribute)
        {
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
                    //draw additional settings associated to the Editor
                    //for example:
                    // - audio management for the AudioClip
                    // - model settings within the Previews
                    using (new EditorGUILayout.HorizontalScope(Style.settingStyle))
                    {
                        editor.OnPreviewSettings();
                    }
                }
            }
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

            //get (or create new) Editor for the current property
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
                InspectorUtility.SetIsEditorExpanded(editor, true);

                //TODO: additional scope to handle widths
                //prevent custom Editors for overriding
                var labelWidth = EditorGUIUtility.labelWidth;
                var fieldWidth = EditorGUIUtility.fieldWidth;

                using (new EditorGUILayout.VerticalScope(Style.inlinedStyle))
                {
                    //draw and prewarm the inlined Editor   
                    DrawEditor(editor, attribute);
                }

                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUIUtility.fieldWidth = fieldWidth;
            }
        }


        /// <summary>
        /// Handles data clearing between Editors.
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
            internal static readonly GUIStyle settingStyle;

            static Style()
            {
                inlinedStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(13, 13, 8, 8)
                };
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 10,
#else
                    fontSize = 9,
#endif
                    alignment = TextAnchor.MiddleLeft
                };

                previewStyle = new GUIStyle();
                previewStyle.normal.background = EditorGuiUtility.CreateColorTexture();

                settingStyle = new GUIStyle()
                {
#if UNITY_2019_3_OR_NEWER
                    padding = new RectOffset(4, 0, 0, 0)
#else
                    padding = new RectOffset(1, 0, 0, 0)
#endif
                };
            }
        }
    }
}