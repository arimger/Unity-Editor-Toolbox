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
        public void TestExtractionPass(string propertyPath, string source, object expected)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var sibling = property.GetSibling(source);
            var fieldInfo = sibling.GetFieldInfo();
            sibling.SetProperValue(fieldInfo, expected, false);
            ValueExtractionHelper.TryGetValue(source, property, out var actual, out var hasMixedValues);
            Assert.IsFalse(hasMixedValues);
            Assert.AreEqual(expected, actual);
        }
    }
}