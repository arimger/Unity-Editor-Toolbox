using System;

using UnityEditor;

namespace Toolbox.Editor
{
    using Object = UnityEngine.Object;
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// Base editor class.
    /// </summary>
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ToolboxEditor : Editor
    {
        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override sealed void OnInspectorGUI()
        {
            OnBeginToolboxEditor?.Invoke(this);
            DrawCustomInspector();
            OnAfterToolboxEditor?.Invoke(this);
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
            if (ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                var isExpanded = true;

                serializedObject.Update();

                var property = serializedObject.GetIterator();
                if (property.NextVisible(isExpanded))
                {
                    isExpanded = false;

                    var isScript = PropertyUtility.IsDefaultScriptProperty(property);

                    EditorGUI.BeginDisabledGroup(isScript);
                    EditorGUILayout.PropertyField(property);
                    EditorGUI.EndDisabledGroup();

                    while (property.NextVisible(isExpanded))
                    {
                        DrawCustomProperty(property.Copy());
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                DrawDefaultInspector();
            }
        }


        public static event Action<Editor> OnBeginToolboxEditor;
        public static event Action<Editor> OnAfterToolboxEditor;
    }
}