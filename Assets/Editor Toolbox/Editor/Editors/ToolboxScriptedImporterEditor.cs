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
            Drawer.DrawEditor(serializedObject);
            if (extraDataType != null)
            {
                Drawer.DrawEditor(extraDataSerializedObject);
            }

            ApplyRevertGUI();
        }

        public void IgnoreProperty(SerializedProperty property)
        {
            Drawer.IgnoreProperty(property);
        }

        public void IgnoreProperty(string propertyPath)
        {
            Drawer.IgnoreProperty(propertyPath);
        }


        Editor IToolboxEditor.ContextEditor => this;
        public IToolboxEditorDrawer Drawer { get; } = new ToolboxEditorDrawer();
    }
}