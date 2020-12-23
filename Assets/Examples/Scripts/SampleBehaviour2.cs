using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 2")]
public class SampleBehaviour2 : MonoBehaviour
{
    [Help("This sample component provides additional inspector extensions (drawers and associated attributes) implemented in the Editor Toolbox plugin. " +
          "Check the SampleBehaviour2.cs script for more details.", Order = -1)]

    [Label("Toolbox Attributes", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter, Content = "UnityEditor.InspectorWindow")]

    [Label("1", skinStyle: SkinStyle.Box)]

    [ReorderableList(ListStyle.Boxed, elementLabel: "GameObject"), Tooltip("Sample List")]
    public GameObject[] list;

    [Label("2", skinStyle: SkinStyle.Box)]

    [BeginGroup("Parent group")]
    public int y;
    [BeginGroup("Nested group")]
    public int var14;
    [Separator] //or [Line]
    public int var15;
    public int var16;
    [BeginIndent]
    public int var17;
    public int var18;
    [EndIndent]
    public int var19;
    [EndGroup]
    [Line]
    [Line(HexColor = "#9800FF")]
    public int var20;
    [EndGroup]
    public int x;

    [Label("3", skinStyle: SkinStyle.Box)]

    [InLineEditor]
    public Transform var21;

    [InLineEditor(drawSettings: true)]
    public Material var22;

    [InLineEditor(true, true)]
    public Texture var23;

    [InLineEditor(drawSettings: true)]
    public AudioClip var24;

    [InLineEditor]
    public Mesh var25;

    [InLineEditor(drawPreview: true)]
    public GameObject var26;

    [Label("4", skinStyle: SkinStyle.Box)]

    [Disable]
    public int[] vars1 = new[] { 1, 2, 3, 4 };

    [System.Serializable]
    public class SampleNestedClass
    {
        public int i = 0;
        [DisableIf(nameof(i), 1), ReorderableList, TagSelector]
        public string[] strings;
    }

    [Label("5", skinStyle: SkinStyle.Box)]

    public SampleNestedClass var27;

    [Label("6", skinStyle: SkinStyle.Box)]

    [Highlight(0, 1, 0)]
    public GameObject var28;

    [Label("7", skinStyle: SkinStyle.Box)]

    [BeginHorizontal]
    public int var29;
    public int var30;
    //NOTE: custom sample created within the Examples
    //[Sample]
    [EndHorizontal]
    public int var31;

    [Label("8", skinStyle: SkinStyle.Box)]

    [ImageArea("https://img.itch.zone/aW1nLzE5Mjc3NzUucG5n/original/Viawjm.png", 180.0f)]
    public int var1;

    [Label("9", skinStyle: SkinStyle.Box)]

    public string stringValue = "sho";
    [ShowIf(nameof(stringValue), "show")] //or HideIf
    public int var33;

    [SpaceArea]

    public KeyCode enumValue = KeyCode.A;
    [DisableIf(nameof(enumValue), KeyCode.A)] //or EnableIf
    public int var35;

    [Label("10", skinStyle: SkinStyle.Box)]

    [ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
    public GameObject[] largeArray = new GameObject[19];

    [Label("11", skinStyle: SkinStyle.Box)]

    [DisableInPlayMode]
    public int var36;
}