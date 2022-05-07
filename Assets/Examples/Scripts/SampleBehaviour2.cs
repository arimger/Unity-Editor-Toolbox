using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 2 (Toolbox Property)")]
public class SampleBehaviour2 : MonoBehaviour
{
    [Label("Toolbox Property Attributes", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter, Asset = "UnityEditor.InspectorWindow")]

    [Label("ReorderableList", skinStyle: SkinStyle.Box)]

    [ReorderableList(ListStyle.Round, elementLabel: "GameObject"), Tooltip("Sample List")]
    [InLineEditor]
    public GameObject[] list;

    [ReorderableListExposed(OverrideNewElementMethodName = nameof(GetValue))]
    public int[] ints;

    public int GetValue()
    {
        return ints.Length * Random.Range(1, 5);
    }

    [Label("InLineEditor", skinStyle: SkinStyle.Box)]

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

    [Label("Nested Properties", skinStyle: SkinStyle.Box)]

    [Help("You can use Toolbox Properties inside serializable types without limitations.")]
    public SampleNestedClass var27;

    [System.Serializable]
    public class SampleNestedClass
    {
        public int i = 0;
        [DisableIf(nameof(i), 1), ReorderableList, TagSelector]
        public string[] strings;
    }

    [Label("ScrollableItems", skinStyle: SkinStyle.Box)]

    [ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
    public GameObject[] largeArray = new GameObject[19];

    [Label("Ignore Parent", skinStyle: SkinStyle.Box)]

    [IgnoreParent]
    public Quaternion q;

    [Label("Dynamic Range & MinMaxSlider", skinStyle: SkinStyle.Box)]

    public float a1 = -1;
    public float b1 = 5.5f;
    [DynamicRange(nameof(a1), nameof(b1))]
    public float var40;
    [DynamicMinMaxSlider(nameof(a1), nameof(b1))]
    public Vector2 var41;
}