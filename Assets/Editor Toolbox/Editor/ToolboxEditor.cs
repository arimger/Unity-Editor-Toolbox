using System.Collections.Generic;

using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Base Editor class for all Toolbox-related features.
    /// </summary>
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ToolboxEditor : Editor, IToolboxEditor
    {
        private readonly HashSet<string> propertiesToIgnore = new HashSet<string>();

        //TODO: better initialization
        private IToolboxEditor nestedEditor;

        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            nestedEditor ??= new BasicToolboxEditor(this);
            ToolboxEditorHandler.HandleToolboxEditor(this);
        }

        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>.
        /// </summary>
        /// <param name="property">Property to display.</param>
        public virtual void DrawCustomProperty(SerializedProperty property)
        {
            nestedEditor.DrawCustomProperty(property);
        }

        /// <summary>
        /// Draws each available property using internally <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector()
        {
            nestedEditor.DrawCustomInspector();
        }

        /// <summary>
        /// Draws each available property using internally <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector(SerializedObject serializedObject)
        {
            nestedEditor.DrawCustomInspector(serializedObject);
        }

        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        public void IgnoreProperty(SerializedProperty property)
        {
            IgnoreProperty(property.propertyPath);
        }

        /// <summary>
        /// Forces associated <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        public void IgnoreProperty(string propertyPath)
        {
            propertiesToIgnore.Add(propertyPath);
        }

        Editor IToolboxEditor.ContextEditor => this;
    }
}