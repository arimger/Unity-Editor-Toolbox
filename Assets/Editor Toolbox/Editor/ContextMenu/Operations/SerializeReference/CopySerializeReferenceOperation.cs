using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class CopySerializeReferenceOperation : IContextMenuOperation
    {
        internal static CopySerializeReferenceCache Cache { get; private set; }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Cache = null;
        }

        public bool IsVisible(SerializedProperty property)
        {
#if UNITY_2021_3_OR_NEWER
            return property != null && property.propertyType == SerializedPropertyType.ManagedReference;
#else
            return false;
#endif
        }

        public bool IsEnabled(SerializedProperty property)
        {
            return true;
        }

        public void Perform(SerializedProperty property)
        {
#if UNITY_2021_3_OR_NEWER
            var value = property.managedReferenceValue;
            if (value != null)
            {
                var referenceType = value.GetType();
                var data = JsonUtility.ToJson(value);
                Cache = new CopySerializeReferenceCache(referenceType, data);
                return;
            }

            Cache = new CopySerializeReferenceCache(null, null);
#endif
        }

        public GUIContent Label => new GUIContent("Copy Serialize Reference");
    }
}