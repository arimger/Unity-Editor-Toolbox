#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "sample")]
public class SampleScriptedImporter : ScriptedImporter
{
    [SerializeField]
    private float sampleFloat;
    [SerializeField, InLineEditor]
    private SampleScriptableObject sampleScriptableObject;

    public override void OnImportAsset(AssetImportContext ctx)
    { }
}
#endif