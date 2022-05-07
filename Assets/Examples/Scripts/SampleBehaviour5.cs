using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[AddComponentMenu("Editor Toolbox/Cheat Sheet 5 (Serialized Types)")]
public class SampleBehaviour5 : MonoBehaviour
{
    [Label("Serialized Type", skinStyle: SkinStyle.Box)]

    [TypeConstraint(typeof(MonoBehaviour), AllowAbstract = false, AllowObsolete = false, TypeGrouping = TypeGrouping.ByAddComponentMenu)]
    public SerializedType type1;
    [TypeConstraint(typeof(IMaskable), AddTextSearchField = true)]
    public SerializedType type2;
    [TypeConstraint(typeof(IMaskable), AddTextSearchField = true, TypeSettings = TypeSettings.Interface)]
    public SerializedType type3;

    [Label("Serialized Scene", skinStyle: SkinStyle.Box)]

    public SerializedScene scene1;
    [SceneDetails]
    public SerializedScene scene2;

    [Label("Serialized Dictionary", skinStyle: SkinStyle.Box)]

#if UNITY_2020_1_OR_NEWER
    [Help("Assign dedicated drawer in the Toolbox Settings")]
    public SerializedDictionary<int, GameObject> dictionary;
#endif

    [Label("Serialized DateTime", skinStyle: SkinStyle.Box)]

    public SerializedDateTime date;
}