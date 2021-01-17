using Toolbox.Editor;
using Toolbox.Editor.Internal;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SampleBehaviour2))]
public class SampleEditor : ToolboxEditor
{
    private SerializedProperty listProperty;

    private ReorderableList list;
    private ReorderableList2 list2;

    private void OnEnable()
    {
        listProperty = serializedObject.FindProperty("list");
        list = new ReorderableList(listProperty, true);
        list2 = new ReorderableList2(listProperty, true);
    }

    private void OnDisable()
    { }

    public override void DrawCustomInspector()
    {
        //base.DrawCustomInspector();

        //for custom properties:
        //ToolboxEditorGui.DrawToolboxProperty(serializedObject.FindProperty("myProperty"));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        serializedObject.Update();
        list.DoLayoutList();
        EditorGUILayout.TextField("AAAAA", "AAAAAA");
        EditorGUILayout.ObjectField(new GUIContent("AAA"), target, typeof(Component), true);
        list2.DoList();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("This label is created in the custom Editor. You can freely extend Toolbox-based Editors by inheriting from the <b>ToolboxEditor</b> class.", Style.labelStyle);
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