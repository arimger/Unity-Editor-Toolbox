using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Examples
{
    public class AttributesExample2 : MonoBehaviour
    {
        [ReorderableList(ListStyle.Boxed)]
        public List<int> reorderableList = new List<int>() { 1, 2, 3 };

        [Group("Sample Group")]
        public string var1;
        [Group("Sample Group"), ReadOnly]
        public string var2 = "Im read only";

        [BoxedHeader("Sample Header")]
        public string var3;

        [Group("Sample Group")]
        public float var4;
       
        [Help("Use this toggle to enable/disable property.", UnityMessageType.Info)]
        [BoxedToggle]
        public bool toggle;

        [DrawIf("toggle", true), ProgressBar]
        public float progressBar = 43;

        public NestedClass serializable;


        [System.Serializable]
        public class NestedClass
        {
            [ReorderableList(ListStyle.Boxed)]
            public List<int> reorderableList;
        }
    }
}