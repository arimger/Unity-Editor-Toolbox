using System.Collections;
using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 2")]
public class SampleBehaviour2 : MonoBehaviour
{
    private void TestMethod()
    {
        Debug.Log(nameof(TestMethod) + " is called");
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

    [EditorButton(nameof(TestMethod), Tooltip = "Custom Tooltip")]
    [EditorButton(nameof(TestCoroutine), "<b>Test Coroutine</b>", activityType: ButtonActivityType.OnPlayMode)]
    [EditorButton(nameof(TestStaticMethod), activityType: ButtonActivityType.OnEditMode)]

    [Help("This sample component provides additional inspector extensions (drawers and associated attributes) implemented in the Editor Toolbox plugin. " +
          "Check the SampleBehaviour2.cs script for more details.", Order = -1)]

    [Label("Toolbox Attributes", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter, Asset = "UnityEditor.InspectorWindow")]

    [Label("1", skinStyle: SkinStyle.Box)]

    [ReorderableList(ListStyle.Round, elementLabel: "GameObject"), Tooltip("Sample List")]
    [InLineEditor]
    public GameObject[] list;

    [Label("2", skinStyle: SkinStyle.Box)]

    [BeginGroup("Parent group")]
    public int y;
    [BeginGroup("Nested group")]
    public int var14;
    [Line]
    public int var15;
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

    [Label("3", skinStyle: SkinStyle.Box)]

    [InLineEditor(DisableEditor = false)]
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

    [BeginHorizontal(labelToWidthRatio: 0.1f)]
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
    [ShowIf(nameof(GetStringValue), "show")]
    public int var33;

    public string GetStringValue()
    {
        return stringValue;
    }

    [SpaceArea]

    public KeyCode enumValue = KeyCode.A;
    [DisableIf(nameof(enumValue), KeyCode.A)]
    public int var35;

    [SpaceArea]

    public GameObject objectValue;
    [HideIf(nameof(ObjectValue), false)]
    public int var36;

    private GameObject ObjectValue
    {
        get => objectValue;
    }

    [SpaceArea]

    public float floatValue = 1.0f;
    [EnableIf(nameof(floatValue), 0.5f, Comparison = UnityComparisonMethod.Greater)]
    public int var37;

    [Label("10", skinStyle: SkinStyle.Box)]

    [ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
    public GameObject[] largeArray = new GameObject[19];

    [Label("11", skinStyle: SkinStyle.Box)]

    [DisableInPlayMode]
    public int var38;

#if UNITY_2020_1_OR_NEWER
    [Label("12", skinStyle: SkinStyle.Box)]

    [Help("Assign dedicated drawer in the Toolbox Settings")]
    public SerializedDictionary<int, GameObject> dictionary;
#endif

    [Label("13", skinStyle: SkinStyle.Box)]

    [ReorderableListExposed(OverrideNewElementMethodName = nameof(GetValue))]
    public int[] ints;

    public int GetValue()
    {
        return ints.Length * Random.Range(1, 5);
    }

    [Label("14", skinStyle: SkinStyle.Box)]

    [IgnoreParent]
    public Quaternion q;

    [Label("15", skinStyle: SkinStyle.Box)]

    [BeginHorizontalGroup(label: "Horizontal Group")]
    [ReorderableList(Foldable = true), InLineEditor]
    public GameObject[] gameObjects;
    [SpaceArea]
    [EndHorizontalGroup]
    [ReorderableList]
    public float[] floats;

    [Label("16", skinStyle: SkinStyle.Box)]

    [DynamicHelp(nameof(MessageSource))]
    public int var39;

    public string MessageSource => "Dynamic Message Source";
}