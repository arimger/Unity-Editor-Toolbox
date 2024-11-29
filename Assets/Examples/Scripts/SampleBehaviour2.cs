using System;
using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 2 (Toolbox Property)")]
public class SampleBehaviour2 : MonoBehaviour
{
    [Label("Toolbox Property Attributes", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter, Asset = "UnityEditor.InspectorWindow")]

    [Label("Reorderable List", skinStyle: SkinStyle.Box)]

    [Tooltip("Sample List")]
    [ReorderableList(ListStyle.Round, elementLabel: "GameObject", Foldable = true)]
    [InLineEditor]
    public GameObject[] list;

    [ReorderableList(ListStyle.Lined, "String", true, false)]
    public string[] strings;

    [ReorderableListExposed(OverrideNewElementMethodName = nameof(GetValue))]
    public int[] ints;

    public int GetValue()
    {
        return ints.Length * UnityEngine.Random.Range(1, 5);
    }

    [Label("InLine Editor", skinStyle: SkinStyle.Box)]

    [InLineEditor(DisableEditor = false)]
    public Component component;

    [InLineEditor(drawSettings: true)]
    public Material material;

    [InLineEditor(true, true)]
    public Texture texture;

    [InLineEditor(drawSettings: true)]
    public AudioClip audioClip;

    [InLineEditor(HideScript = true)]
    public Mesh mesh;

    [Label("Scrollable Items", skinStyle: SkinStyle.Box)]

    [ScrollableItems(defaultMinIndex: 0, defaultMaxIndex: 5)]
    public GameObject[] largeArray = new GameObject[19];

    [Label("Ignore Parent", skinStyle: SkinStyle.Box)]

    public Quaternion quaternion;
    [IgnoreParent]
    public Quaternion q2;

    [Label("Dynamic Range & MinMax Slider", skinStyle: SkinStyle.Box)]

    public float min = -1;
    public float max = 5.5f;
    [DynamicRange(nameof(min), nameof(max))]
    public float dynamicRange;
    [DynamicMinMaxSlider(nameof(min), nameof(max))]
    public Vector2 dynamicMinMax;

    [Label("Nested Objects", skinStyle: SkinStyle.Box)]

    [Help("You can use Toolbox Attributes inside serializable types without limitations.")]
    public SampleNestedClass nestedObject;

    [Serializable]
    public class SampleNestedClass
    {
        [Tooltip("Set to 1")]
        public int i = 0;
        [DisableIf(nameof(i), 1), ReorderableList, TagSelector]
        [Help("Nested Information.", ApplyCondition = true)]
        public string[] strings;
    }
}