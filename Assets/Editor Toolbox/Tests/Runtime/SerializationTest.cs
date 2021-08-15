using NUnit.Framework;
using System;
using UnityEngine;
using UnityEditor;

namespace Toolbox.Tests
{
    public class SerializationTest
    {
        [TestCase("UnityEngine.MonoBehaviour, UnityEngine.CoreModule", typeof(MonoBehaviour))]
        [TestCase("UnityEngine.ScriptableObject, UnityEngine.CoreModule", typeof(ScriptableObject))]
        [TestCase("UnityEngine.GameObject, UnityEngine.CoreModule", typeof(GameObject))]
        [TestCase("Some random string", null)]
        [TestCase("Toolbox.Tests.SerializationTest, Toolbox.Tests", typeof(SerializationTest))]
        public void TestTypeSerializationPass(string typeString, Type expectedType)
        {
            var serializedType = new SerializedType(typeString);
            var type = serializedType.Type;
            Assert.AreEqual(type, expectedType);
        }

#if UNITY_2020_1_OR_NEWER
        [Test]
        public void TestDictSerializationPass()
        {
            var so = ScriptableObject.CreateInstance<TestScriptableObject>();
            var serializedObject = new SerializedObject(so);
            var dict = serializedObject.FindProperty("dict");
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

            Assert.AreEqual(so.dict.Count, 2);
            Assert.IsTrue(so.dict.ContainsKey(0));
            Assert.IsTrue(so.dict.ContainsKey(1));
            Assert.IsFalse(so.dict.ContainsKey(2));
            Assert.AreEqual(so.dict[1], "B");
        }
#endif
    }
}