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

        private static string GenerateKey(SerializedProperty property)
        {
            return property.serializedObject.GetHashCode() + "-" + property.name;
        }


        private static Dictionary<string, UnityEditor.Editor> editorInstances = new Dictionary<string, UnityEditor.Editor>();

        #endregion


        public override void OnGui(SerializedProperty property, InLineEditorAttribute attribute)
        {
            EditorGUILayout.PropertyField(property, property.isExpanded);

            //TODO: arrays support
            if (property.isArray)
                return;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on reference value properties.");      
                return;
            }

            if (property.objectReferenceValue == null)
                return;

            var key = GenerateKey(property);

            if (!editorInstances.TryGetValue(key, out UnityEditor.Editor editor))
            {
                editorInstances[key] = editor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);
            }

            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent("Inspector Preview"), true, Style.foldoutStyle))
            {
                editor.serializedObject.Update();
                EditorGUI.indentLevel++;
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
                editor.serializedObject.ApplyModifiedProperties();
            }
        }


        private static class Style
        {
            internal static readonly GUIStyle foldoutStyle;

            static Style()
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    //fontStyle = FontStyle.Bold,
                    fontSize = 9,
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }
    }
}