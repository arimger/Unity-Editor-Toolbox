using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    public class PropertyUtilitesTest
    {
        private SerializedObject scriptableObject;


        [OneTimeSetUp]
        public void SetUp()
        {
            var target = ScriptableObject.CreateInstance<TestObject2>();
            scriptableObject = new SerializedObject(target);
            var array = scriptableObject.FindProperty("var2.var3");
            array.InsertArrayElementAtIndex(0);
        }

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
        [TestCase("var2.var3.Array.data[0]", 1)]
        public void TestGetValuePass(string propertyPath, object value)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var fieldInfo = property.GetFieldInfo(out _, scriptableObject.targetObject);
            property.SetProperValue(fieldInfo, value);
            var newValue = property.GetProperValue(fieldInfo);
            Assert.AreEqual(value, newValue);
        }

        [TestCase("var2.var2.var1", false)]
        [TestCase("var2.var3", false)]
        [TestCase("var2.var3.Array.data[0]", true)]
        public void TestIsArrayElementPass(string propertyPath, bool expected)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var actual = PropertyUtility.IsSerializableArrayElement(property);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("var1", "var1")]
        [TestCase("var2.var1", "var2.var1")]
        [TestCase("var2.var2.var1", "var2.var2.var1")]
        [TestCase("var2.var3", "var2.var3")]
        [TestCase("var2.var3.Array.data[0]", "var2.var3.[0]")]
        public void TestGetPathTreePass(string propertyPath, string path)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var pathTree = PropertyUtility.GetPropertyFieldTree(property);
            var actual = string.Join(".", pathTree);
            Assert.AreEqual(path, actual);
        }

        [TestCase("m_Script", true)]
        [TestCase("var2.var1", false)]
        public void TestIsMonoScriptPass(string propertyPath, bool expected)
        {
            var property = scriptableObject.FindProperty(propertyPath);
            var actual = PropertyUtility.IsDefaultScriptProperty(property);
            Assert.AreEqual(expected, actual);
        }
    }
}