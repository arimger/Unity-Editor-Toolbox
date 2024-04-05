using System.Collections;

using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 4 (Decorator)")]
public class SampleBehaviour4 : MonoBehaviour
{
    [Label("Help", skinStyle: SkinStyle.Box)]

    [Disable]
    [Help("Very useful warning", UnityMessageType.Warning)]
    [Help("This error example", UnityMessageType.Error, ApplyCondition = true)]
    [Help("Simple information", UnityMessageType.Info)]
    public int var0;

    [Label("Button", skinStyle: SkinStyle.Box)]

    [EditorButton(nameof(TestMethod), Tooltip = "Custom Tooltip", ValidateMethodName = nameof(ValidationMethod))]
    [EditorButton(nameof(TestCoroutine), "<b>Test Coroutine</b>", activityType: ButtonActivityType.OnPlayMode)]
    [EditorButton(nameof(TestStaticMethod), activityType: ButtonActivityType.OnEditMode)]
    public int var1;

    private void TestMethod()
    {
        Debug.Log(nameof(TestMethod) + " is called");
    }

    private bool ValidationMethod()
    {
        return var1 == 0;
    }

    private IEnumerator TestCoroutine()
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSecondsRealtime(1);
        Debug.Log("Log after 1s");
        yield return new WaitForSecondsRealtime(2);
        Debug.Log("Log after 2s");
    }

    private static void TestStaticMethod()
    {
        Debug.Log(nameof(TestStaticMethod) + " is called");
    }

    [Label("Vertical Layout", skinStyle: SkinStyle.Box)]

    [BeginGroup("Parent group")]
    public int y;
    [BeginGroup("Nested group")]
    public int var14;
    [Line]
    public int var15;
    [SpaceArea(20, 20)]
    public int var16;
    [BeginIndent]
    public int var17;
    public int var18;
    [Title("Standard Header")]
    public GameObject go;
    [Label("<color=red><b>Custom Header</b></color>")]
    [EndIndent]
    public int var19;
    [EndGroup]
    [Line]
    [Line(HexColor = "#9800FF")]
    public int var20;
    [EndGroup]
    public int x;

    [Label("Horizontal Layout", skinStyle: SkinStyle.Box)]

    [BeginHorizontal(LabelWidth = 50.0f)]
    public int var29;
    [SpaceArea(10)]
    public int var30;
    [EndHorizontal]
    public int var31;

    [Label("Horizontal Layout (Group)", skinStyle: SkinStyle.Box)]

    [BeginHorizontalGroup(Label = "Horizontal Group", ControlFieldWidth = true, ElementsInLayout = 2)]
    [ReorderableList(Foldable = true), InLineEditor]
    public GameObject[] gameObjects;
    [SpaceArea]
    [EndHorizontalGroup]
    [ReorderableList]
    public float[] floats;

    [Label("Indentation", skinStyle: SkinStyle.Box)]

    public int var2;
    [BeginIndent]
    public int var3;
    [EndIndent]
    public int var4;
    [IndentArea(3)]
    public int var5;

    [Label("Highlight", skinStyle: SkinStyle.Box)]

    [Highlight(0.8f, 1.0f, 0.2f)]
    public GameObject var28;

    [Label("Dynamic Help", skinStyle: SkinStyle.Box)]

    [DynamicHelp(nameof(MessageSource))]
    public int var39;

    public string MessageSource => string.Format("Dynamic Message Source. {0} = {1}", nameof(var39), var39);

    [Label("Image Area", skinStyle: SkinStyle.Box)]

    [ImageArea("https://img.itch.zone/aW1nLzE5Mjc3NzUucG5n/original/Viawjm.png", 180.0f)]
    public int var55;

    [Label("GUI Color", skinStyle: SkinStyle.Box)]

    [GuiColor(1, 0, 0)]
    public int var56;

    [Label("Title", skinStyle: SkinStyle.Box)]

    [Title("Standard Title")]
    public int var57;
}