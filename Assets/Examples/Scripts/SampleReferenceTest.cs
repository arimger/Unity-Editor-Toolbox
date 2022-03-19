using UnityEngine;

public class SampleReferenceTest : MonoBehaviour
{
    public bool isOn;
    [SerializeReference]
    public SampleReferenceBase reference;

    private void OnValidate()
    {
        if (isOn)
        {
            reference = new SampleReference1();
        }
        else
        {
            reference = new SampleReference2();
        }
    }
}