using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    /// <summary>
    /// Base editor class.
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Object), true, isFallback = true)]
    public class ToolboxEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Handles desired property display process using <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void DrawCustomProperty(SerializedProperty property)
        {
            //ToolboxEditorGui class will handle all properties and drawers
            ToolboxEditorGui.DrawLayoutToolboxProperty(property);
        }

        /// <summary>
        /// Draws custom inspector using <see cref="ToolboxDrawer"/>s.
        /// </summary>
        protected virtual void DrawCustomInspector()
        {
            //begin iteration over all properties
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                //handle situation when first property is not mono script
                if (property.propertyPath != Defaults.defaultScriptPropertyPath)
                {
                    DrawDefaultInspector();
                    return;
                }

                //begin Toolbox inspector
                EditorGUILayout.LabelField(Defaults.defaultEditorHeaderText, EditorStyles.centeredGreyMiniLabel);

                //draw standard script property
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property);
                EditorGUI.EndDisabledGroup();

                serializedObject.Update();
                //draw every property using Toolbox drawers
                while (property.NextVisible(false))
                {
                    DrawCustomProperty(property.Copy());
                }
                serializedObject.ApplyModifiedProperties();
            }
        }


        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //draw default inspector if ToolboxDrawers are not allowed
            if (!ToolboxSettingsUtility.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector();
                return;
            }

            //draw custom inspector using additionally custom ToolboxDrawers
            DrawCustomInspector();
        }


        internal static class Defaults
        {
            internal const string defaultEditorHeaderText = "Component Editor";

            internal const string defaultScriptPropertyPath = "m_Script";

            internal const string defaultScriptPropertyType = "PPtr<MonoScript>";
        }
    }
}