using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    public interface IToolboxEditor
    {
        void DrawCustomInspector();
        void IgnoreProperty(SerializedProperty property);
        void IgnoreProperty(string propertyPath);

        /// <summary>
        /// <see cref="Editor"/> instance associated to this Editor.
        /// </summary>
        Editor ContextEditor { get; }
        /// <summary>
        /// Dedicated "drawer" that is responsible for default drawing strategy for this <see cref="ToolboxEditor"/>.
        /// </summary>
        IToolboxEditorDrawer Drawer { get; }
    }
}