using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Examples
{
    public class AttributesExample1 : MonoBehaviour
    {
        [OrderedSpace(25, 20)]
        [OrderedLine]
        [NewLabel("Item", "Element")]
        [Group("Custom group"), ReorderableList(ListStyle.Lined)]
        public List<int> linedStyleList;

        [ReorderableList(ListStyle.Round, fixedSize: true)]
        public string[] standardStyleList = new string[4];

        [Help("I'm a little bit indented.")]
        [Group("Custom group"), Indent]
        public string textInGroup;

        [Group("Another group"), AssetPreview(useLabel: false)]
        public GameObject asset;
        [Group("Another group"), AssetPreview]
        public GameObject asset1;

        [Group("Custom group"), ReadOnly]
        public int intInGroup;

        [Help("Use this toggle to enable/disable property.", UnityMessageType.Warning)]
        [Help("Use this toggle to enable/disable property.", UnityMessageType.Error), HideLabel]
        public bool isTrue;

        [Space, Separator, Space]

        [DrawIf("isTrue", true), ProgressBar]
        public float x = 43;

        [Group("Useful Popups"), TagSelector]
        public string tagSelector;

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

        [Group("Useful Popups"), ClassExtends(typeof(Object))]
        public SerializedTypeReference baseClassReference;
        [Group("Useful Popups"), ClassImplements(typeof(ICollection))]
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

        [Group("Useful Popups"), EnumFlag]
        public FlagExample enumFlag = FlagExample.Flag1 | FlagExample.Flag2;

        [NotNull]
        public Transform transformRequired;


#if UNITY_EDITOR
        private void OnValidate()
        {
            asset = asset1;
        }
#endif
    }
}