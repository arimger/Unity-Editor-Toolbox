using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    using Toolbox.Editor.Internal;

    public class ReorderableListTest
    {
        private SerializedObject scriptableObject;

        private ToolboxEditorList list;


        [OneTimeSetUp]
        public void SetUp()
        {
            var target = ScriptableObject.CreateInstance<TestObject2>();
            scriptableObject = new SerializedObject(target);
            var array = scriptableObject.FindProperty("var2.var3");
            list = new ToolboxEditorList(array);
        }

        [Test]
        public void TestGetSizePass()
        {
            var size = list.Size;
            size.intValue = 0;
            list.AppendElement();
            list.AppendElement();
            list.AppendElement();
            list.RemoveElement();
            list.AppendElement();
            Assert.AreEqual(3, size.intValue);
        }
    }
}