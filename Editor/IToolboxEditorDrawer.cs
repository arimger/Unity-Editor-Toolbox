using UnityEditor;

namespace Toolbox.Editor
{
    public interface IToolboxEditorDrawer
    {
        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>.
        /// </summary>
        /// <param name="property">Property to display.</param>
        void DrawToolboxProperty(SerializedProperty property);
        /// <summary>
        /// Handles property display process using the default (built-in) drawers.
        /// </summary>
        /// <param name="property">Property to display.</param>
        void DrawDefaultProperty(SerializedProperty property);
        /// <summary>
        /// Draws each available property using internally <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        void DrawToolboxInspector(SerializedObject serializedObject);
        /// <summary>
        /// Draws <see cref="SerializedProperty"/>/ies in the default (native) way.
        /// </summary>
        /// <param name="serializedObject"></param>
        void DrawDefaultInspector(SerializedObject serializedObject);
        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        void IgnoreProperty(SerializedProperty property);
        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        void IgnoreProperty(string propertyPath);
    }
}