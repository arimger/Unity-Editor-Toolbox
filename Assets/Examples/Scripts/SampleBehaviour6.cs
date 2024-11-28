using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 6 (Serialize Reference)")]
public class SampleBehaviour6 : MonoBehaviour
{
#if UNITY_2019_3_OR_NEWER
    [Label("Standard Types", skinStyle: SkinStyle.Box)]
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.ByFlatName)]
    public ISampleInterface var1;
    [SerializeReference, ReferencePicker(ForceUninitializedInstance = true)]
    public ClassWithInterfaceBase var2;
    [SerializeReference, ReferencePicker]
    public ClassWithInterfaceBase var3;
    [SerializeReference, ReferencePicker(ParentType = typeof(ClassWithInterface2), AddTextSearchField = false)]
    public ClassWithInterfaceBase var4;
    [SerializeReference, ReferencePicker, ReorderableList]
    public ISampleInterface[] vars;
#endif

#if UNITY_2023_2_OR_NEWER
    [Label("Generic Types", skinStyle: SkinStyle.Box)]
    [SerializeReference, ReferencePicker(TypeGrouping = TypeGrouping.None)]
    public IGenericInterface<string> generic;
#endif

    #region Standard Types

    public interface ISampleInterface
    { }

    [Serializable]
#if UNITY_2019_3_OR_NEWER
    [MovedFrom(false, null, null, "SampleBehaviour6/Struct")]
#endif
    public struct SampleStruct : ISampleInterface
    {
        public bool var1;
        public bool var2;

        public SampleStruct(bool var1, bool var2)
        {
            this.var1 = var1;
            this.var2 = var2;
        }
    }

    public abstract class ClassWithInterfaceBase : ISampleInterface
    { }

    [Serializable]
    public class ClassWithInterface1 : ClassWithInterfaceBase
    {
        [InLineEditor]
        public GameObject go;
#if UNITY_2019_2_OR_NEWER
        [SerializeReference, ReferencePicker]
#endif
        public ISampleInterface var1;
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

        public ClassWithInterface3(int var1)
        {
            this.var1 = var1;
        }
    }

    [Serializable]
    public class ClassWithInterface4 : ClassWithInterface2
    {
        public int var33;
    }

    #endregion

    #region Generic Types

    public interface IGenericInterface<TValue>
    {
        TValue Value { get; }
    }

    public class IntInterfaceImplementationInt : IGenericInterface<int>
    {
        [SerializeField]
        private int value;

        public int Value => value;
    }

    public class StringInterfaceImplementation : IGenericInterface<string>
    {
        [SerializeField]
        private string value;

        public string Value => value;
    }

    public class GenericInterfaceImplementation<TValue> : IGenericInterface<TValue>
    {
        [SerializeField]
        private TValue value;
        [SerializeField]
        private bool var1;

        public TValue Value => value;
    }

    public class WrongConstraintGenericInterfaceImplementation<TValue> : IGenericInterface<TValue> where TValue : struct
    {
        [SerializeField]
        private TValue value;
        [SerializeField]
        private float var1;

        public TValue Value => value;
    }

    #endregion
}