using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Toolbox.Editor.Drawers
{
    public class InLineEditorAttributeDrawer : ToolboxPropertyDrawer<InLineEditorAttribute>
    {
        /// <summary>
        /// Collection of all stored <see cref="UnityEditor.Editor"/> instances.
        /// </summary>
        private static Dictionary<string, UnityEditor.Editor> editorInstances = new Dictionary<string, UnityEditor.Editor>();


        /// <summary>
        /// Draws inlined version of <see cref="UnityEditor.Editor"></see> and handles all unexpected situations.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void HandleEditorPrewarm(UnityEditor.Editor editor, InLineEditorAttribute attribute)
        {
            if (!attribute.DrawHeader)
            {
                //force expanded inspector if header is not expected
                if (!InternalEditorUtility.GetIsInspectorExpanded(editor.target))
                {
                    InternalEditorUtility.SetIsInspectorExpanded(editor.target, true);
#if !UNITY_2019_1_OR_NEWER
                    const string isVisibleFieldName = "m_IsVisible";
                    //in older versions editor's foldouts are based on m_IsVisible field and Awake() method
                    var isVisible = editor.GetType().GetField(isVisibleFieldName,
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    if (isVisible != null)
                    {
                        isVisible.SetValue(editor, true);
                    }
#endif
                }
            }

            //prevent custom editors for overriding label width
            var labelWidth = EditorGUIUtility.labelWidth;
            HandleEditorDrawing(editor, attribute);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /// <summary>
        /// Draws inlined editor using provided <see cref="UnityEditor.Editor"/> object.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void HandleEditorDrawing(UnityEditor.Editor editor, InLineEditorAttribute attribute)
        {
            //draw header if needed
            if (attribute.DrawHeader)
            {
                editor.DrawHeader();
            }

            //begin editor inside vertical group
            EditorGUILayout.BeginVertical(Style.inlinedStyle);
         
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
            }
  
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Handles property drawing process and tries to create inlined version of <see cref="UnityEditor.Editor"/>
        /// for <see cref="UnityEngine.Object"/> associated to this property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, InLineEditorAttribute attribute)
        {
            EditorGUILayout.PropertyField(property, label, property.isExpanded);

            //basically multiple values are not supported yet
            if (property.hasMultipleDifferentValues || property.objectReferenceValue == null)
            {
                return;
            }

            var key = property.GetPropertyKey();
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

            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent(property.objectReferenceValue.GetType().Name + " Inspector Preview"), true, Style.foldoutStyle))
            {                
                //draw and prewarm inlined editor   
                HandleEditorPrewarm(editor, attribute);
            }
        }


        /// <summary>
        /// Handles data clearing between editors.
        /// </summary>
        public override void OnGuiReload()
        {
            //clear all obsolete editors
            editorInstances.Clear();
        }

        /// <summary>
        /// Checks if provided property can by handled by this drawer.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        /// <summary>
        /// Custom style representation.
        /// </summary>
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

                //create background texture for all previews
                var backgroundTex = new Texture2D(1, 1);
                backgroundTex.SetPixel(0, 0, new Color(0, 0, 0, 0));
                backgroundTex.Apply();
                backgroundTex.hideFlags = HideFlags.HideAndDontSave;

                //create preview style based on transparent texture
                previewStyle = new GUIStyle();
                previewStyle.normal.background = backgroundTex;
            }
        }
    }
}