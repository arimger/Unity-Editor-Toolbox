using UnityEditor;
using UnityEditor.AssetImporters;

namespace Toolbox.Editor.Editors
{
    using Editor = UnityEditor.Editor;

    public class ToolboxScriptedImporterEditor : ScriptedImporterEditor, IToolboxEditor
    {
        public sealed override void OnInspectorGUI()
        {
            ToolboxEditorHandler.HandleToolboxEditor(this);
        }

        public virtual void DrawCustomInspector()
        {
            Drawer.DrawToolboxInspector(serializedObject);
            if (extraDataType != null)
            {
                Drawer.DrawToolboxInspector(extraDataSerializedObject);
            }

            ApplyRevertGUI();
        }

        //TODO: ignore properties
        public void IgnoreProperty(SerializedProperty property)
        { }

        public void IgnoreProperty(string propertyPath)
        { }

        Editor IToolboxEditor.ContextEditor => this;
        public IToolboxEditorDrawer Drawer { get; } = new ToolboxEditorDrawer();
    }
}