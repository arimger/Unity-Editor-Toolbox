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
            if (!ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector();
            }
            else
            {
                DrawCustomInspector();
            }            
        }


        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        public virtual void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorGui.DrawLayoutToolboxProperty(property);
        }

        /// <summary>
        /// Draws each available property using custom <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector()
        {
            var expanded = true;

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            if (property.NextVisible(expanded))
            {
                expanded = false;

                var disable = InspectorUtility.IsDefaultScriptProperty(property);

                EditorGUI.BeginDisabledGroup(disable);
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