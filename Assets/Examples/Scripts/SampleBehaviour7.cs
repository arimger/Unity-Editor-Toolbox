using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 7 (Special & Others)")]
public class SampleBehaviour7 : MonoBehaviour
{
    [Label("NewLabel", skinStyle: SkinStyle.Box)]

    [NewLabel("float")]
    public float var1 = 25.4f;
    [NewLabel("My custom label")]
    [InLineEditor]
    public Transform var2;

    [Label("HideLabel", skinStyle: SkinStyle.Box)]

    [HideLabel]
    public int var3;
}