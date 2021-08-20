using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    public class SerializationTest
    {
#if UNITY_2020_1_OR_NEWER
        [Test]
        public void TestDictSerializationPass()
        {
            var so = ScriptableObject.CreateInstance<TestObject1>();
            var serializedObject = new SerializedObject(so);
            var dict = serializedObject.FindProperty("var1");
            var list = dict.FindPropertyRelative("pairs");
            SerializedProperty pair;

            list.InsertArrayElementAtIndex(0);
            pair = list.GetArrayElementAtIndex(0);
            pair.FindPropertyRelative("key").intValue = 0;
            pair.FindPropertyRelative("value").stringValue = "A";
            list.InsertArrayElementAtIndex(1);
            pair = list.GetArrayElementAtIndex(1);
            pair.FindPropertyRelative("key").intValue = 1;
            pair.FindPropertyRelative("value").stringValue = "B";
            list.InsertArrayElementAtIndex(2);
            pair = list.GetArrayElementAtIndex(2);
            pair.FindPropertyRelative("key").intValue = 1;
            pair.FindPropertyRelative("value").stringValue = "C";

            serializedObject.ApplyModifiedProperties();

            var reference = so.var1;
            Assert.AreEqual(reference.Count, 2);
            Assert.AreEqual(reference.ContainsKey(0), true);
            Assert.AreEqual(reference.ContainsKey(1), true);
            Assert.AreEqual(reference.ContainsKey(2), false);
            Assert.AreEqual(so.var1[1], "B");
        }
#endif
    }
}