#if UNITY_2021_3_OR_NEWER
using System.Collections.Generic;

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
            Reset();
        }

        private CopySerializeReferenceEntry CreateEntry(SerializedProperty property)
        {
            if (property == null)
            {
                return new CopySerializeReferenceEntry(null, null);
            }

            var value = property.managedReferenceValue;
            if (value == null)
            {
                return new CopySerializeReferenceEntry(null, null);
            }

            var referenceType = value.GetType();
            var data = JsonUtility.ToJson(value);
            return new CopySerializeReferenceEntry(referenceType, data);
        }

        internal static void Reset()
        {
            Cache = null;
        }

        public bool IsVisible(SerializedProperty property)
        {
            return PropertyUtility.IsSerializeReferenceProperty(property);
        }

        public bool IsEnabled(SerializedProperty property)
        {
            return property.isArray ? property.arraySize > 0 : true;
        }

        public void Perform(SerializedProperty property)
        {
            var entries = new List<CopySerializeReferenceEntry>();
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                var entry = CreateEntry(property);
                entries.Add(entry);
            }
            else if (property.isArray)
            {
                var propertiesCount = property.arraySize;
                for (var i = 0; i < propertiesCount; i++)
                {
                    var childProperty = property.GetArrayElementAtIndex(i);
                    var entry = CreateEntry(childProperty);
                    entries.Add(entry);
                }
            }

            PropertyUtility.TryGetSerializeReferenceType(property, out var referenceType);
            Cache = new CopySerializeReferenceCache(referenceType, entries);
        }

        public GUIContent Label => new GUIContent("Copy Serialized References");
    }
}
#endif