using UnityEngine;

public class SampleReferenceTest : MonoBehaviour
{
    [SerializeReference, ReferencePicker]
    public SampleReferenceBase reference;
}