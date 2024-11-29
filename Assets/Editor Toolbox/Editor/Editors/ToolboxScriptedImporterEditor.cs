using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Toolbox.Editor.Editors
{
    using Toolbox.Editor.Drawers;
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
#if UNITY_2020_2_OR_NEWER
            if (extraDataType != null)
            {
                Drawer.DrawEditor(extraDataSerializedObject);
            }
#endif
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