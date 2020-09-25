using System;

using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

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
            OnCloseToolboxEditor?.Invoke(this);
        }


        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="property">Property to display.</param>
        public virtual void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        /// <summary>
        /// Draws each available property using custom <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector()
        {
            if (ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                serializedObject.Update();

                var isExpanded = true;

                var property = serializedObject.GetIterator();
                //enter to the 'Base' property
                if (property.NextVisible(isExpanded))
                {
                    isExpanded = false;
                    var script = PropertyUtility.IsDefaultScriptProperty(property);

                    //try to draw the first property (m_Script)
                    using (new EditorGUI.DisabledScope(script))
                    {
                        DrawCustomProperty(property.Copy());
                    }

                    //iterate over all other serialized properties
                    //NOTE: every child will be handled internally
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
        public static event Action<Editor> OnCloseToolboxEditor;
    }
}