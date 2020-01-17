using UnityEngine;

[ExecuteAlways]
public class SampleBehaviour : MonoBehaviour
{
    [Help("This sample component provides all additional inspector extensions(drawers and associated attributes) implemented in Editor Toolbox plugin. " +
          "Check SampleBehaviour.cs file for more details.", order = -1)]

    [BoxedHeader("1")]

    [Help("You can provide more information in HelpBoxes.", order = 100)]
    public int var1;

    [BoxedHeader("2")]

    [TagSelector]
    public string targetTag;

    [BoxedHeader("3")]

    [ProgressBar(minValue:10.0f, maxValue:50.0f)]
    public float progressBar = 25.4f;

    [BoxedHeader("4")]

    [NewLabel("float")]
    public float newLabel = 25.4f;

    [BoxedHeader("5")]

    [MinMaxSlider(10.0f, 100.0f)]
    public Vector2 var2;

    [BoxedHeader("6")]

    [Indent(1)]
    public int var3 = 1;
    [Indent(2)]
    public int var4 = 2;
    [Indent(3)]
    public int var5 = 3;

    [BoxedHeader("7")]
    
    [HideLabel, Help("Use this toggle to show/hide property.", order = 100)]
    public bool toggle1;
    [ConditionalHide("toggle1", true)]
    public float var6;

    [BoxedHeader("8")]

    [HideLabel, Help("Use this toggle to enable/disable property.", order = 100)]
    public bool toggle2;
    [ConditionalDisable(nameof(toggle2), true)]
    public float var7;

    [BoxedHeader("9")]

    [InstanceButton(typeof(SampleBehaviour), nameof(ResetVar8), 
                                        "Use this button to reset var8 using this instance[in Play mode]", ButtonActivityType.OnPlayMode, order = 100)]
    [BroadcastButton(nameof(ResetVar8), "Use this button to reset var8 using broadcasting[in Edit mode]", ButtonActivityType.OnEditMode, order = 100)]

    [AssetPreview]
    public GameObject var8;
    [AssetPreview(useLabel:false), Help("Who needs label?")]
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

    [BoxedHeader("10")]

    [Suffix("kg")]
    public float var10;

    [BoxedHeader("11")]

    [ClassExtends(typeof(Object))]
    public SerializedType type1;
    [ClassImplements(typeof(System.Collections.ICollection))]
    public SerializedType type2;

    [BoxedHeader("12")]

    [ReadOnlyField]
    public string var11 = "Im read only";

    [BoxedHeader("13")]

    [BoxedToggle]
    public bool var12;

    [BoxedHeader("14")]

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

    [BoxedHeader("15")]

    [NotNull]
    public Transform var13;

    [BoxedHeader("16")]

    [Random(-10.0f, 10.0f)]
    public float randomValue;

    [BoxedHeader("17")]

    [Directory]
    public string directory;

    [BoxedHeader("18")]

    [SceneName]
    public string sceneName;

    [BoxedHeader("19")]

    [Preset(nameof(presetValues))]
    public int presetTarget;

    private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

    [BoxedHeader("20")]

    [SearchableEnum]
    public KeyCode enumSearch;

    [BoxedHeader("21")]

    [Clamp(0.0f, 11.2f)]
    public double clampedValue;

    [HeaderArea("Toolbox Attributes", HeaderStyle.Boxed)]
    [HeaderArea("1", HeaderStyle.Boxed)]

    [ReorderableList(ListStyle.Boxed, elementLabel:"GameObject"), Tooltip("Sample List")]
    public GameObject[] list;

    [HeaderArea("2", HeaderStyle.Boxed)]

    [BeginGroup("Custom group")]
    public int var14;
    [Separator]
    public int var15;
    public int var16;
    [BeginIndent]
    public int var17;
    public int var18;
    [EndIndent]
    public int var19;
    [EndGroup]
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

    [InLineEditor]
    public AudioClip var24;

    [SpaceArea]

    [InLineEditor]
    public Mesh var25;

    [InLineEditor(drawHeader: true, drawPreview: true)]
    public GameObject var26;

    [HeaderArea("4", HeaderStyle.Boxed)]

    [Disable]
    public int[] vars1 = new[] {1, 2, 3, 4};


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
    [EndHorizontal]
    public int var31;

    private void Start()
    {
        transform.hideFlags = HideFlags.HideInInspector;
    }
}