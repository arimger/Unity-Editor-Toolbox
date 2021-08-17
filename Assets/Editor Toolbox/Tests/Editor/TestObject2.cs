using System;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    internal class TestObject2 : ScriptableObject
    {
        public int var1;
        public TestNestedObject1 var2;

        [Serializable]
        public class TestNestedObject1
        {
            public string var1;
            public TestNestedObject2 var2;
            public float[] var3;
        }

        [Serializable]
        public class TestNestedObject2
        {
            public bool var1;
            public int var2;
        }
    }
}