using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 1")]
public class SampleBehaviour1 : MonoBehaviour
{
    [Help("This sample component provides additional inspector extensions (drawers and associated attributes) implemented in the Editor Toolbox plugin. " +
          "Check the SampleBehaviour1.cs script for more details.", Order = -1)]

    [BoxedHeader("1")]

    [Help("You can provide more information in HelpBoxes.", Order = 100)]
    public int var1;

    [BoxedHeader("2")]

    [TagSelector]
    public string targetTag;

    [BoxedHeader("3")]

    [ProgressBar(minValue:10.0f, maxValue:50.0f, Color = "#234DEA")]
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
    
    [HideLabel]
    public bool toggle1;
    [Help("Use this toggle to show/hide property.", Order = 100)]
    [ConditionalHide(nameof(toggle1), true)] //ConditionalShow
    public float var6;

    [BoxedHeader("8")]

    [HideLabel]
    public bool toggle2;
    [Help("Use this toggle to enable/disable property.", Order = 100)]
    [ConditionalDisable(nameof(toggle2), true)] //ConditionalEnable
    public float var7;

    [BoxedHeader("9")]

    [InstanceButton(typeof(SampleBehaviour1), nameof(ResetVar8), 
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

    [ClassExtends(typeof(Object)), Tooltip("This variable is able to serialize Type.")]
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

    [Preset(nameof(presetValues)), Tooltip("Pick value")]
    public int presetTarget;

    private readonly int[] presetValues = new[] { 1, 2, 3, 4, 5 };

    [BoxedHeader("20")]

    [SearchableEnum]
    public KeyCode enumSearch;

    [BoxedHeader("21")]

    [Clamp(0.0f, 11.2f)]
    public double clampedValue;

    [BoxedHeader("22")]

    [Password]
    public string password;

    [BoxedHeader("23")]

    [Vector2Range(0, 1)]
    public Vector2 vector2;
    [Vector3Range(0, 1)]
    public Vector3 vector3;

    [BoxedHeader("24")]

    [PrefabReference]
    public GameObject prefab;

    [BoxedHeader("25")]

    [HexColor]
    public string hexColor;

    [BoxedHeader("26")]

    [Vector3Direction]
    public Vector3 direction3d;
    [Vector2Direction]
    public Vector2 direction2d;
}