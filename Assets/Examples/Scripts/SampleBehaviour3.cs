using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 3 (Condition)")]
public class SampleBehaviour3 : MonoBehaviour
{
    [Label("Show If", skinStyle: SkinStyle.Box)]

    [Help("Type 'show'")]
    public string stringValue = "sho";
    [ShowIf(nameof(GetStringValue), "show")]
    public int var33;

    public string GetStringValue()
    {
        return stringValue;
    }

    [Label("Hide If", skinStyle: SkinStyle.Box)]

    [Help("Assign any GameObject")]
    public GameObject objectValue;
    [HideIf(nameof(ObjectValue), false)]
    public int var36;

    private GameObject ObjectValue
    {
        get => objectValue;
    }

    [Label("Enable If", skinStyle: SkinStyle.Box)]

    [Help("Set value to > 0.5")]
    public float floatValue = 1.0f;
    [EnableIf(nameof(floatValue), 0.5f, Comparison = UnityComparisonMethod.Greater)]
    public int var37;

    [Label("Disable If", skinStyle: SkinStyle.Box)]

    public KeyCode enumValue = KeyCode.A;
    [DisableIf(nameof(enumValue), KeyCode.A)]
    public int var35;

    [Label("Disable", skinStyle: SkinStyle.Box)]

    [Disable]
    public int[] var41= new int[4];

    [Label("Disable In Playmode", skinStyle: SkinStyle.Box)]

    [DisableInPlayMode]
    public int var39;

    [Label("Show Warning If", skinStyle: SkinStyle.Box)]

    [ShowWarningIf(nameof(var40), 3, "Value has to be greater than 3", Comparison = UnityComparisonMethod.LessEqual, DisableField = false)]
    public int var40;

    [Label("Show Disabled If", skinStyle: SkinStyle.Box)]

    [ShowDisabledIf(nameof(var40), 3, Comparison = UnityComparisonMethod.LessEqual)]
    public int var42;

    [Label("Hide Disabled If", skinStyle: SkinStyle.Box)]

    [HideDisabledIf(nameof(var40), 3, Comparison = UnityComparisonMethod.GreaterEqual)]
    public int var43;
}