using NUnit.Framework;
using System;
using UnityEngine;

namespace Toolbox.Tests
{
    public class SerializationTest
    {
        public class TestObject
        {
#if UNITY_2020_1_OR_NEWER
            public SerializedDictionary<int, string> var1;
#endif
        }

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
            var instance = new TestObject();
            var reference = new SerializedDictionary<int, string>();
            instance.var1 = reference;
            reference.Add(1, "A");
            reference.Add(2, "B");
            reference.Add(3, "C");

            Assert.AreEqual(reference.Count, 3);
            Assert.AreEqual(reference.ContainsKey(1), true);
            Assert.AreEqual(reference.ContainsKey(2), true);
            Assert.AreEqual(reference.ContainsKey(3), true);
            Assert.AreEqual(reference[2], "B");
        }
#endif
    }
}