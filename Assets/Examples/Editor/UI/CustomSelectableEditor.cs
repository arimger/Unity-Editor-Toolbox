using Toolbox.Editor;
using Toolbox.Editor.Drawers;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomSelectable))]
public class CustomSelectableEditor : SelectableEditor, IToolboxEditor
{
    private static readonly string[] selectableProperties = new string[]
    {
        "m_Script",
        "m_Interactable",
        "m_TargetGraphic",
        "m_Transition",
        "m_Colors",
        "m_SpriteState",
        "m_AnimationTriggers",
        "m_Navigation"
    };

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var property in selectableProperties)
        {
            IgnoreProperty(property);
        }
    }

    public sealed override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ToolboxEditorHandler.HandleToolboxEditor(this);
    }

    public void DrawCustomInspector()
    {
        Drawer.DrawEditor(serializedObject);
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