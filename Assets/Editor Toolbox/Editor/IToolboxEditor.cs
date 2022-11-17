using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    public interface IToolboxEditor
    {
        void DrawCustomInspector();
        void IgnoreProperty(SerializedProperty property);
        void IgnoreProperty(string propertyPath);

        Editor ContextEditor { get; }
        IToolboxEditorDrawer Drawer { get; }
    }
}