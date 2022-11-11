using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;

    public interface IToolboxEditor
    {
        void DrawCustomProperty(SerializedProperty property);
        void DrawCustomInspector();
        void DrawCustomInspector(SerializedObject serializedObject);

        Editor ContextEditor { get; }
    }
}