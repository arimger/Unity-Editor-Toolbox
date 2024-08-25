using System;
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

    [Label("LabelByChild", skinStyle: SkinStyle.Box)]

    [LabelByChild("var3.var2")]
    public SampleClass1 sampleField;
    [LabelByChild("var2"), ReorderableList, Tooltip("Sample usage of the LabelByChildAttribute within a list")]
    public SampleClass1[] sampleFields;

    [Serializable]
    public class SampleClass1
    {
        public Material var1;
        public KeyCode var2;
        public SampleClass2 var3;
    }

    [Serializable]
    public class SampleClass2
    {
        public int var1;
        public string var2;
    }
}