using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    using Toolbox.Editor.Drawers;

    public class ExtractionTest
    {
        private SerializedObject scriptableObject;


        [OneTimeSetUp]
        public void SetUp()
        {
            var target = ScriptableObject.CreateInstance<TestObject2>();
            scriptableObject = new SerializedObject(target);
        }

        [TestCase("var2", "var1", 3)]
        [TestCase("var2.var3", "var1", "TestValue")]
        [TestCase("var2.var2.var2", "var1", true)]
        public void TestFieldsExtractionPass(string propertyPath, string source, object expected)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var sibling = property.GetSibling(source);
            var fieldInfo = sibling.GetFieldInfo();
            sibling.SetProperValue(fieldInfo, expected, false);
            ValueExtractionHelper.TryGetValue(source, property, out var actual, out var hasMixedValues);
            Assert.IsFalse(hasMixedValues);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("var2", "BoolValue", true)]
        [TestCase("var2.var3", "GetFloatValue", 1.6f)]
        [TestCase("var2.var2.var2", "IntValue", 3)]
        public void TestPropertiesExtractionPass(string propertyPath, string source, object expected)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            ValueExtractionHelper.TryGetValue(source, property, out var actual, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("var2", "GetStringValue", "Text", true)]
        [TestCase("var2", "GetDoubleValue", null, false)]
        [TestCase("var2.var3", "GetFloatValue", 1.6f, true)]
        [TestCase("var2.var2.var2", "GetIntValue", null, false)]
        [TestCase("var2.var2.var2", "DoSomething", null, true)]
        public void TestMethodsExtractionPass(string propertyPath, string source, object expected, bool success)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var isFound = ValueExtractionHelper.TryGetValue(source, property, out var actual, out _);
            Assert.AreEqual(success, isFound);
            if (isFound)
            {
                Assert.AreEqual(expected, actual);
            }
        }
    }
}