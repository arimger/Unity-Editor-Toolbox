using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

public class AttributesExample : MonoBehaviour
{
    /*****************************************
     * Sample use of every custom attribute:
     *
     *      Standard property drawers(property drawer-based):
     *          - Help
     *          - TagSelector
     *          - Separator
     *          - BoxedHeader
     *          - BoxedToggle
     *          - ProgressBar
     *          - NewLabel
     *          - MinMaxSlider
     *          - Indent
     *          - ConditionalField
     *          - AssetPreview
     *          - HideLabel
     *          - Suffix
     *          - TypeConstraint(ClassExtends or ClassImplements attribute)
     *          - ReadOnlyField
     *          - EnumFlag
     *          - NotNull
     *          
     *      Ordered property drawers(custom editor-based):
     *          - Group
     *          - ReorderableList
     *          - DrawIf
     *          - ReadOnly
     *          - Button
     *          - OrderedSpace
     *          - OrderedLine
     ******************************************/

    [OrderedSpace(25, 20)]
    [OrderedLine]
    [NewLabel("Item", "Element")]
    [Group("Custom group"), ReorderableList(ListStyle.Lined)]
    public List<int> linedStyleList;

    [ReorderableList(ListStyle.Round)]
    public List<string> standardStyleList;

    [ReorderableList(ListStyle.Boxed, fixedSize: true)]
    public GameObject[] boxedStyleList = new GameObject[4];

    [Help("I'm a little bit indented.")]
    [Group("Custom group"), Indent]
    public string textInGroup;

    [Group("Another group"), AssetPreview(justPreview: true)]
    public GameObject asset;
    [Group("Another group"), AssetPreview]
    public GameObject asset1;

    [Group("Custom group"), ReadOnly]
    public int intInGroup;

    [Help("Use this toggle to enable/disable property.", UnityMessageType.Warning)]
    [Help("Use this toggle to enable/disable property.", UnityMessageType.Error), HideLabel]
    public bool isTrue;

    [DrawIf("isTrue", true), ProgressBar]
    public float x = 43;

    [Space, Separator, Space]

    [TagSelector]
    public string tagHere;

    [BoxedHeader("Really nice header")]

    [NewLabel("Vector3")]
    public Vector3 iNeedNewName;

    [BoxedToggle]
    public bool imImportantBool;

    [MinMaxSlider(0, 100)]
    public Vector2 minMaxValues;

    [Suffix("cm")]
    public int suffixNeededHere;

    [Suffix("Prefab")]
    public GameObject prefab;

    [ClassExtends(typeof(Object))]
    public SerializedTypeReference baseClassReference;
    [ClassImplements(typeof(ICollection))]
    public SerializedTypeReference interfaceReference;

    [ReadOnlyField]
    public string imReadOnly = "Read only text";

    [Flags]
    public enum FlagExample
    {
        Nothing = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4,
        Everything = ~0
    }

    [EnumFlag]
    public FlagExample flagExample = FlagExample.Flag1 | FlagExample.Flag2;

    [NotNull]
    public Transform transformRequired;

    [Button("Button")]
    public void ButtonExample()
    {
        Debug.Log("Pressed!");
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        asset = asset1;
    }
#endif
}