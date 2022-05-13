using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 1 (Regular)")]
public class SampleBehaviour1 : MonoBehaviour
{
    [Label("Tag Selector", skinStyle: SkinStyle.Box)]

    [TagSelector]
    public string targetTag;

    [Label("Progress Bar", skinStyle: SkinStyle.Box)]

    [ProgressBar(minValue: -10.0f, maxValue: 50.0f, HexColor = "#234DEA")]
    public float progressBar = 25.4f;

    [Label("MinMax Slider", skinStyle: SkinStyle.Box)]

    [MinMaxSlider(10.0f, 100.0f)]
    public Vector2 var2;

    [Label("Asset Preview", skinStyle: SkinStyle.Box)]

    [AssetPreview]
    public GameObject var8;
    [AssetPreview]
    public Transform preview;

    [Label("Suffix", skinStyle: SkinStyle.Box)]

    [Suffix("kg")]
    public float var10;

    [Label("Left Toggle", skinStyle: SkinStyle.Box)]

    [LeftToggle]
    public bool var12;

    [Label("Enum Toggles", skinStyle: SkinStyle.Box)]

    [EnumToggles]
    public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2;

    [System.Flags]
    public enum FlagExample
    {
        Nothing = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4,
        Flag4 = 8,
        Flag5 = 16,
        Flag6 = 32,
        Flag7 = 64,
        Flag8 = 128,
        Flag9 = 256,
        Everything = ~0
    }

    [Label("Directory", skinStyle: SkinStyle.Box)]

    [Directory]
    public string directory;

    [Label("Scene Name", skinStyle: SkinStyle.Box)]

    [SceneName]
    public string sceneName;

    [Label("Preset", skinStyle: SkinStyle.Box)]

    [Preset(nameof(presetValues), nameof(optionLabels)), Tooltip("Pick value")]
    public int presetTarget;

    private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };
    private readonly string[] optionLabels = new[] { "a", "b", "c", "d", "e" };

    [Label("Searchable Enum", skinStyle: SkinStyle.Box)]

    [SearchableEnum]
    public KeyCode enumSearch;

    [Label("Password", skinStyle: SkinStyle.Box)]

    [Password]
    public string password;

    [Label("Validation", skinStyle: SkinStyle.Box)]

    [Help("NotNullAttribute, ClampAttribute, SceneObjectOnlyAttribute, ChildObjectOnlyAttribute, PrefabObjectOnlyAttribute " +
        "are part of group that will be re-implemented in future as ToolboxValidationAttributes. " +
        "Unfortunately, for now, you can't use them together with any other PropertyDrawer.", UnityMessageType.Warning)]
    [NotNull]
    public Transform var13;

    [Clamp(0.0f, 11.2f)]
    public double clampedValue;

    [SceneObjectOnly]
    public GameObject sceneReference;
    [ChildObjectOnly]
    public GameObject childReference;
    [PrefabObjectOnly]
    public GameObject prefabReference;

    [Label("Label By Child", skinStyle: SkinStyle.Box)]

    [LabelByChild("var3.var2")]
    public SampleClass1 sampleField;
    [LabelByChild("var2")]
    public SampleClass1[] sampleFields;

    [System.Serializable]
    public class SampleClass1
    {
        public Material var1;
        public KeyCode var2;
        public SampleClass2 var3;
    }

    [System.Serializable]
    public class SampleClass2
    {
        public int var1;
        public string var2;
    }

    [Label("Formatted Number", skinStyle: SkinStyle.Box)]

    [FormattedNumber]
    public int bigNumber;
    [FormattedNumber("c")]
    public float currency;
}