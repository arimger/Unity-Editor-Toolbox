using System;

using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 6 (Serialize Reference)")]
public class SampleBehaviour6 : MonoBehaviour
{
#if UNITY_2019_3_OR_NEWER
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    public Interface1 var1;
    [SerializeReference, ReferencePicker]
    public ClassWithInterfaceBase var2;
    [SerializeReference, ReferencePicker(ParentType = typeof(ClassWithInterface2))]
    public ClassWithInterfaceBase var3;
    [SerializeReference, ReferencePicker]
    public Interface1[] vars;
#endif

    public interface Interface1
    { }

    [Serializable]
    public struct Struct : Interface1
    {
        public bool var1;
        public bool var2;
    }

    public abstract class ClassWithInterfaceBase : Interface1
    { }

    [Serializable]
    public class ClassWithInterface1 : ClassWithInterfaceBase
    {
        [InLineEditor]
        public GameObject go;
#if UNITY_2019_2_OR_NEWER
        [SerializeReference, ReferencePicker]
#endif
        public Interface1 var1;
    }

    [Serializable]
    public class ClassWithInterface2 : ClassWithInterfaceBase
    {
        [LeftToggle]
        public bool var1;
        [InLineEditor]
        public Material mat;
    }

    [Serializable]
    public class ClassWithInterface3 : ClassWithInterfaceBase
    {
        public int var1;
    }

    [Serializable]
    public class ClassWithInterface4 : ClassWithInterface2
    {
        public int var33;
    }
}