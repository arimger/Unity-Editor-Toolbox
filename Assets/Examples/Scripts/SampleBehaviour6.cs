using System;

using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 6 (Serialize Reference)")]
public class SampleBehaviour6 : MonoBehaviour
{
    [SerializeReference, ReferencePicker]
    public Interface1 var1;
    [SerializeReference, ReferencePicker]
    public ClassWithInterfaceBase var2;

    public interface Interface1 { }

    [Serializable]
    public struct Struct : Interface1
    {
        public bool var1;
        public bool var2;
    }

    public abstract class ClassWithInterfaceBase : Interface1 { }

    [Serializable]
    public class ClassWithInterface1 : ClassWithInterfaceBase
    {
        public GameObject go;
    }

    [Serializable]
    public class ClassWithInterface2 : ClassWithInterfaceBase
    {
        [LeftToggle]
        public bool var1;
    }

    [Serializable]
    public class ClassWithInterface3 : ClassWithInterfaceBase
    {
        public int var1;
    }
}