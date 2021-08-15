using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    public class PropertyUtilitesTest
    {
        private class TestObject : ScriptableObject
        {
            public int var1;
            public TestNestedObject1 var2;

            [Serializable]
            public class TestNestedObject1
            {
                public string var1;
                public TestNestedObject2 var2;
            }

            [Serializable]
            public class TestNestedObject2
            {
                public bool var1;
            }
        }


        private SerializedObject scriptableObject;


        [OneTimeSetUp]
        public void SetUp()
        {
            var target = ScriptableObject.CreateInstance<TestObject>();
            scriptableObject = new SerializedObject(target);
        }

        [OneTimeTearDown]
        public void TearDown()
        { }

        [TestCase("var1", null)]
        [TestCase("var2.var1", "var2")]
        [TestCase("var2.var2.var1", "var2")]
        public void TestGetParentPass(string propertyPath, string parentName)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var parent = property?.GetParent();
            Assert.AreEqual(parent?.name, parentName);
        }

        [TestCase("var1", 3)]
        [TestCase("var2.var1", "TestValue")]
        [TestCase("var2.var2.var1", true)]
        public void TestGetValuePass(string propertyPath, object value)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var fieldInfo = property.GetFieldInfo(out var type, scriptableObject.targetObject);
            property.SetProperValue(fieldInfo, value);
            var newValue = property.GetProperValue(fieldInfo);
            Assert.AreEqual(value, newValue);
        }
    }
}