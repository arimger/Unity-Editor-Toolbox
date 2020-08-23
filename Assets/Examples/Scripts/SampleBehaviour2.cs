using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 2")]
public class SampleBehaviour2 : MonoBehaviour
{
    [Help("This sample component provides additional inspector extensions (drawers and associated attributes) implemented in the Editor Toolbox plugin. " +
          "Check the SampleBehaviour2.cs script for more details.", Order = -1)]

    [HeaderArea("Toolbox Attributes", HeaderStyle.Boxed)]
    [HeaderArea("1", HeaderStyle.Boxed)]

    [ReorderableList(ListStyle.Boxed, elementLabel: "GameObject"), Tooltip("Sample List")]
    public GameObject[] list;

    [HeaderArea("2", HeaderStyle.Boxed)]

    [BeginGroup("Custom group")]
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
    [Line()]
    [Line(Color = "#9800FF")]
    public int var20;

    [HeaderArea("3", HeaderStyle.Boxed)]

    [InLineEditor]
    public Transform var21;

    [SpaceArea]

    [InLineEditor]
    public Material var22;

    [SpaceArea]

    [InLineEditor(true, true)]
    public Texture var23;

    [SpaceArea]

    [InLineEditor(drawSettings: true)]
    public AudioClip var24;

    [SpaceArea]

    [InLineEditor]
    public Mesh var25;

    [InLineEditor(drawHeader: true, drawPreview: true)]
    public GameObject var26;

    [HeaderArea("4", HeaderStyle.Boxed)]

    [Disable]
    public int[] vars1 = new[] { 1, 2, 3, 4 };

    [System.Serializable]
    public class SampleNestedClass
    {
        public int i = 0;
        [ReorderableList, TagSelector]
        public string[] strings;
    }

    [HeaderArea("5", HeaderStyle.Boxed)]

    public SampleNestedClass var27;

    [HeaderArea("6", HeaderStyle.Boxed)]

    [Highlight(0, 1, 0)]
    public GameObject var28;

    [HeaderArea("7", HeaderStyle.Boxed)]

    [BeginHorizontal]
    public int var29;
    public int var30;
    //NOTE: custom sample created within the Examples
    //[Sample]
    [EndHorizontal]
    public int var31;

    [HeaderArea("8", HeaderStyle.Boxed)]

    [ImageArea("https://img.itch.zone/aW1nLzE5Mjc3NzUucG5n/original/Viawjm.png", 150.0f)]
    public int var32;

    [HeaderArea("9", HeaderStyle.Boxed)]

    public string stringValue = "sho";
    [ShowIf(nameof(stringValue), "show")]
    public int var33;

    [SpaceArea]

    public KeyCode enumValue = KeyCode.A;
    [DisableIf(nameof(enumValue), KeyCode.A)]
    public int var35;
}