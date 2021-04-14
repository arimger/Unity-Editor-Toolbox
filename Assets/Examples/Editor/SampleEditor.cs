using UnityEditor;
using UnityEngine;
using Toolbox.Editor;

[CustomEditor(typeof(SampleBehaviour2))]
public class SampleEditor : ToolboxEditor
{
    private void OnEnable()
    { }

    private void OnDisable()
    { }

    public override void DrawCustomInspector()
    {
        base.DrawCustomInspector();

        //for custom properties:
        //ToolboxEditorGui.DrawToolboxProperty(serializedObject.FindProperty("myProperty"));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("This label is created in the custom Editor. " +
            "You can freely extend Toolbox-based Editors by inheriting from the <b>ToolboxEditor</b> class.", Style.labelStyle);
    }


    private static class Style
    {
        internal static readonly GUIStyle labelStyle;

        static Style()
        {
            labelStyle = new GUIStyle(EditorStyles.helpBox)
            {
                richText = true,
                fontSize = 14
            };
        }
    }
}