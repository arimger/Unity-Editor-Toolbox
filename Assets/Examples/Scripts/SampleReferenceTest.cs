using UnityEngine;

public class SampleReferenceTest : MonoBehaviour
{
    public bool isOn;
    [SerializeReference, ReferencePicker]
    public SampleReferenceBase reference;
}