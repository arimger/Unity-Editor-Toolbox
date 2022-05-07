using UnityEngine;

public class SampleReferenceTest : MonoBehaviour
{
    [TypeConstraint(typeof(Interface1), AddTextSearchField = true)]
    public SerializedType type1;
    [TypeConstraint(typeof(Interface1), TypeSettings = TypeSettings.Interface, AddTextSearchField = true)]
    public SerializedType type2;
    [ClassExtends(typeof(Interface1))]
    public SerializedType type3;

    [SerializeReference, ReferencePicker]
    public SampleReferenceBase reference;
    [SerializeReference, ReferencePicker]
    public SampleReferenceBase reference2;

    public interface Interface1 { }
    public interface Interface2 : Interface1 { }
    public interface Interface3 : Interface1 { }
    public interface Interface4 : Interface2 { }
    public interface Interface4<T> : Interface3 { }

    public class ClassWithInterface : Interface1 { }
}