using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 1")]
public class SampleBehaviour1 : MonoBehaviour
{
    [Help("This sample component provides additional inspector extensions (drawers and associated attributes) implemented in the Editor Toolbox plugin. " +
          "Check the SampleBehaviour1.cs script for more details.", Order = -1)]

    [Label("1", skinStyle: SkinStyle.Box)]

    [Help("You can provide more information in HelpBoxes.", Order = 100)]
    public int var1;

    [Label("2", skinStyle: SkinStyle.Box)]

    [TagSelector]
    public string targetTag;

    [Label("3", skinStyle: SkinStyle.Box)]

    [ProgressBar(minValue: -10.0f, maxValue: 50.0f, HexColor = "#234DEA")]
    public float progressBar = 25.4f;

    [Label("4", skinStyle: SkinStyle.Box)]

    [NewLabel("float")]
    public float newLabel = 25.4f;

    [Label("5", skinStyle: SkinStyle.Box)]

    [MinMaxSlider(10.0f, 100.0f)]
    public Vector2 var2;

    [Label("6", skinStyle: SkinStyle.Box)]

    [Indent(1)]
    public int var3 = 1;
    [Indent(2)]
    public int var4 = 2;
    [Indent(3)]
    public int var5 = 3;

    [Label("7", skinStyle: SkinStyle.Box)]

    [HideLabel]
    public bool toggle1;
    [Help("Use this toggle to show/hide property.", Order = 100)]
    [HideIf(nameof(toggle1), true)]
    public float var6;

    [Label("8", skinStyle: SkinStyle.Box)]

    [HideLabel]
    public bool toggle2;
    [Help("Use this toggle to enable/disable property.", Order = 100)]
    [EnableIf(nameof(toggle2), true)]
    public float var7;

    [Label("9", skinStyle: SkinStyle.Box)]

    [InstanceButton(typeof(SampleBehaviour1), nameof(ResetVar8),
                                        "Use this button to reset var8 using this instance[in Play mode]", ButtonActivityType.OnPlayMode, order = 100)]
    [BroadcastButton(nameof(ResetVar8), "Use this button to reset var8 using broadcasting[in Edit mode]", ButtonActivityType.OnEditMode, order = 100)]

    [AssetPreview]
    public GameObject var8;
    [AssetPreview(useLabel: false), Help("Who needs label?")]
    public GameObject var9;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var9 = var8;
    }
#endif

    private void ResetVar8()
    {
        var8 = null;
        Debug.Log("Var8 resetted");
    }

    [Label("10", skinStyle: SkinStyle.Box)]

    [Suffix("kg")]
    public float var10;

    [Label("11", skinStyle: SkinStyle.Box)]

    [ClassExtends(typeof(Object)), Tooltip("This variable is able to serialize Type.")]
    public SerializedType type1;
    [ClassImplements(typeof(System.Collections.ICollection))]
    public SerializedType type2;

    [Label("12", skinStyle: SkinStyle.Box)]

    [ReadOnlyField]
    public string var11 = "Im read only";

    [Label("13", skinStyle: SkinStyle.Box)]

    [LeftToggle]
    public bool var12;

    [Label("14", skinStyle: SkinStyle.Box)]

    [EnumFlag]
    public FlagExample enumFlag1 = FlagExample.Flag1 | FlagExample.Flag2;
    [EnumFlag(EnumStyle.Button)]
    public FlagExample enumFlag2 = FlagExample.Flag1 | FlagExample.Flag2 | FlagExample.Flag7;

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

    [Label("15", skinStyle: SkinStyle.Box)]

    [NotNull]
    public Transform var13;

    [Label("16", skinStyle: SkinStyle.Box)]

    [Random(-10.0f, 10.0f)]
    public float randomValue;

    [Label("17", skinStyle: SkinStyle.Box)]

    [Directory]
    public string directory;

    [Label("18", skinStyle: SkinStyle.Box)]

    [SceneName]
    public string sceneName;

    [Label("19", skinStyle: SkinStyle.Box)]

    [Preset(nameof(presetValues)), Tooltip("Pick value")]
    public int presetTarget;

    private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

    [Label("20", skinStyle: SkinStyle.Box)]

    [SearchableEnum]
    public KeyCode enumSearch;

    [Label("21", skinStyle: SkinStyle.Box)]

    [Clamp(0.0f, 11.2f)]
    public double clampedValue;

    [Label("22", skinStyle: SkinStyle.Box)]

    [Password]
    public string password;

    [Label("23", skinStyle: SkinStyle.Box)]

    [Vector2Range(0, 1)]
    public Vector2 vector2;
    [Vector3Range(0, 1)]
    public Vector3 vector3;

    [Label("24", skinStyle: SkinStyle.Box)]

    [PrefabReference]
    public GameObject prefab;

    [Label("25", skinStyle: SkinStyle.Box)]

    [HexColor]
    public string hexColor;

    [Label("26", skinStyle: SkinStyle.Box)]

    [Vector3Direction]
    public Vector3 direction3d;
    [Vector2Direction]
    public Vector2 direction2d;
	
	[Label("27", skinStyle: SkinStyle.Box)]
	
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

    [Label("28", skinStyle: SkinStyle.Box)]

    public SerializedScene scene;
}