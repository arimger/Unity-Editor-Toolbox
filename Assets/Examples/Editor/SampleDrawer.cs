using UnityEditor;

using Toolbox.Editor.Drawers;

public class SampleDrawer : ToolboxDecoratorDrawer<SampleAttribute>
{
    protected override void OnGuiEndSafe(SampleAttribute attribute)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Label created in the custom decorator Drawer.");
    }
}
