using UnityEngine;

namespace Toolbox.Editor.Tests
{
    internal class TestObject1 : ScriptableObject
    {
#if UNITY_2020_1_OR_NEWER
        public SerializedDictionary<int, string> var1;
#endif
    }
}