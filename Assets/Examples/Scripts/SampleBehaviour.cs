using UnityEngine;

[ExecuteAlways]
public class SampleBehaviour : MonoBehaviour
{
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
    [ConditionalDisable("toggle2", true)]
    public float var7;

    [BoxedHeader("9")]

    [InstanceButton(typeof(SampleBehaviour), "ResetVar8", "Use this button to reset var8 using instance", ButtonActivityType.OnPlayMode, order = 100)]
    [BroadcastButton("ResetVar8", "Use this button to reset var8 using broadcasting", ButtonActivityType.OnEditMode, order = 100)]

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
    public SerializedTypeReference type1;
    [ClassImplements(typeof(System.Collections.ICollection))]
    public SerializedTypeReference type2;

    [BoxedHeader("12")]

    [ReadOnlyField]
    public string var11 = "Im read only";

    [BoxedHeader("13")]

    [BoxedToggle]
    public bool var12;

    [BoxedHeader("14")]

    [EnumFlag]
    public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2;

    [System.Flags]
    public enum FlagExample
    {
        Nothing = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4,
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

    [ReorderableList(ListStyle.Boxed, elementLabel:"GameObject")]
    public GameObject[] list;

    //[Group("Custom group")]
    //public int var14;
    //[Separator]
    //public int var15;
    //[Group("Custom group")]
    //public int var16;
}