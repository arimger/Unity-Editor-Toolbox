using System;
using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 6 (Serialize Reference)")]
public class SampleBehaviour6 : MonoBehaviour
{
    [SerializeReference, ReferencePicker]
    public Interface1 var1;
    public ClassWithInterface var2;

    public interface Interface1 { }
    public interface Interface2 : Interface1 { }
    public interface Interface3 : Interface1 { }
    public interface Interface4 : Interface2 { }
    public interface Interface4<T> : Interface3 { }

    [Serializable]
    public struct Struct : Interface1
    {
        public int a;
    }

    [Serializable]
    public class ClassWithInterface : Interface1 
    {
        public GameObject go;
    }

    [Serializable]
    public class ClassWithInterface1 : Interface1 
    {
        [LeftToggle]
        public bool a;
    }

    [Serializable]
    public class ClassWithInterface2 : Interface1 
    {
        public int i;
    }
}