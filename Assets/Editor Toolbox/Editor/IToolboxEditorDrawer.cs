using UnityEditor;

namespace Toolbox.Editor
{
    public interface IToolboxEditorDrawer
    {
        void DrawEditor(SerializedObject serializedObject);
        /// <summary>
        /// Draws <see cref="SerializedProperty"/>/ies in the default (native) way.
        /// </summary>
        /// <param name="serializedObject"></param>
        void DrawDefaultEditor(SerializedObject serializedObject);
        /// <summary>
        /// Draws each available property using internally <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        /// <param name="serializedObject"></param>
        void DrawToolboxEditor(SerializedObject serializedObject);
        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        void IgnoreProperty(SerializedProperty property);
        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        void IgnoreProperty(string propertyPath);
        bool IsPropertyIgnored(SerializedProperty property);
        bool IsPropertyIgnored(string propertyPath);
    }
}