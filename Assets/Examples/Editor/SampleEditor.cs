using UnityEditor;

using Toolbox.Editor;

[CustomEditor(typeof(SampleBehaviour))]
public class SampleEditor : ToolboxEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Label created in the custom Editor.");
    }
}
