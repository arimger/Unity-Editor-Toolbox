using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class InLineEditorAttributeDrawer : ToolboxPropertyDrawer<InLineEditorAttribute>
    {
        #region Data persistence handlers 

        [InitializeOnLoadMethod]
        private static void InitializeDrawer()
        {
            ToolboxEditorUtility.onEditorReload += DeinitializeDrawer;
        }

        private static void DeinitializeDrawer()
        {
            editorInstances.Clear();
        }


        private static Dictionary<string, UnityEditor.Editor> editorInstances = new Dictionary<string, UnityEditor.Editor>();

        #endregion


        public override void OnGui(SerializedProperty property, InLineEditorAttribute attribute)
        {
            EditorGUILayout.PropertyField(property, property.isExpanded);

            //basically arrays and multiple values are not supported yet
            if (property.isArray || property.hasMultipleDifferentValues)
                return;

            //reference value type validation
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on reference value properties.");      
                return;
            }

            if (property.objectReferenceValue == null)
                return;

            var key = GenerateKey(property);

            //get (or create) editor for this property
            if (!editorInstances.TryGetValue(key, out UnityEditor.Editor editor))
            {
                editorInstances[key] = editor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);
            }
            //if reference values does not match we have to reset editor
            else if (editor.target != property.objectReferenceValue)
            {
                editorInstances.Remove(key);
                return;
            }

            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent("Inspector Preview"), true, Style.foldoutStyle))
            {
                //draw header if needed
                if (attribute.DrawHeader)
                {
                    editor.DrawHeader();
                }

                //draw whole inspector and apply all changes 
                editor.serializedObject.Update();
                editor.OnInspectorGUI();
                editor.serializedObject.ApplyModifiedProperties();

                if (!editor.HasPreviewGUI()) return;

                //draw preview if possible and needed
                if(attribute.DrawPreview)
                {
                    editor.OnPreviewGUI(EditorGUILayout.GetControlRect(false, attribute.PreviewHeight), Style.previewStyle);
                }
            }
        }


        private static class Style
        {
            internal static readonly GUIStyle foldoutStyle;
            internal static readonly GUIStyle previewStyle;

            static Style()
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontSize = 9,
                    alignment = TextAnchor.MiddleLeft
                };
                previewStyle = new GUIStyle(EditorStyles.helpBox);
            }
        }
    }
}