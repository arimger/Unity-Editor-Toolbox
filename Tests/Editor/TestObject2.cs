using System;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    internal class TestObject2 : ScriptableObject
    {
        public int var1;
        public TestNestedObject1 var2;

        public bool BoolValue
        {
            get => true;
        }

        public string GetStringValue()
        {
            return "Text";
        }

        public double GetDoubleValue(int arg)
        {
            return arg + 2.0005;
        }

        [Serializable]
        public class TestNestedObject1
        {
            public string var1;
            public TestNestedObject2 var2;
            public float[] var3;

            public float GetFloatValue()
            {
                return 1.6f;
            }
        }

        [Serializable]
        public class TestNestedObject2
        {
            public bool var1;
            public int var2;

            public int IntValue
            {
                get => 3;
            }

            public void DoSomething()
            { }
        }
    }
}