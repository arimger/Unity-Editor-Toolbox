using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//TODO: handling children;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    /// <summary>
    /// Base editor class.
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Object), true, isFallback = true)]
    public class ToolboxEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Editor initialization.
        /// </summary>
        protected virtual void OnEnable()
        { }

        /// <summary>
        /// Editor deinitialization.
        /// </summary>
        protected virtual void OnDisable()
        { }

        /// <summary>
        /// Handles desired property display process using <see cref="ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected virtual void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorGui.DrawProperty(property);
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
                if (property.name != Defaults.defaultScriptPropertyName)
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
                //draw every property using ToolboxAttributes&Drawers
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
            if (!ToolboxEditorUtility.ToolboxDrawersAllowed)
            {
                DrawDefaultInspector();
                return;
            }

            DrawCustomInspector();
        }


        internal static class Defaults
        {
            internal const string defaultEditorHeaderText = "Component Editor";

            internal const string defaultScriptPropertyName = "m_Script";

            internal const string defaultScriptPropertyType = "PPtr<MonoScript>";
        }
    }
}