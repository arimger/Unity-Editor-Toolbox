using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    /// <summary>
    /// Base editor class.
    /// </summary>
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ToolboxEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (!ToolboxDrawerUtility.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector();
                return;
            }

            DrawCustomInspector();
        }


        /// <summary>
        /// Handles desired property display process using <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        public virtual void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorGui.DrawLayoutToolboxProperty(property);
        }

        /// <summary>
        /// Draws custom inspector using <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector()
        {
            const string editorHeaderText = "Component Editor";

            var expanded = true;

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            if (property.NextVisible(expanded))
            {
                expanded = false;

                EditorGUILayout.LabelField(editorHeaderText, ToolboxEditorUtility.headerTextStyle);

                EditorGUI.BeginDisabledGroup(property.type == ToolboxEditorUtility.defaultScriptPropertyType);
                EditorGUILayout.PropertyField(property);
                EditorGUI.EndDisabledGroup();

                while (property.NextVisible(expanded))
                {
                    DrawCustomProperty(property.Copy());
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}