#if UNITY_2021_3_OR_NEWER
using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class PasteSerializeReferenceOperation : IContextMenuOperation
    {
        private bool IsAssignmentValid(SerializedProperty targetProperty, IReadOnlyList<CopySerializeReferenceEntry> entires)
        {
            if (!PropertyUtility.TryGetSerializeReferenceType(targetProperty, out var targetType))
            {
                return false;
            }

            for (var i = 0; i < entires.Count; i++)
            {
                var entry = entires[i];
                if (!IsAssignmentValid(targetType, entry))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAssignmentValid(Type targetType, CopySerializeReferenceEntry entry)
        {
            var entryType = entry.ReferenceType;
            if (entryType == null)
            {
                return true;
            }

            return TypeUtility.IsTypeAssignableFrom(targetType, entry.ReferenceType);
        }

        private bool IsOperationSupported(SerializedProperty targetProperty, CopySerializeReferenceCache cache)
        {
            if (cache == null)
            {
                return false;
            }

            var entries = cache.Entries;
            if (entries == null || entries.Count == 0)
            {
                return false;
            }

            if (cache.IsArrayCopy && targetProperty.isArray)
            {
                return true;
            }

            if (!cache.IsArrayCopy && !targetProperty.isArray)
            {
                return true;
            }

            return false;
        }

        private object GetManagedReferenceValue(CopySerializeReferenceEntry entry)
        {
            if (entry.ReferenceType != null)
            {
                var newValue = JsonUtility.FromJson(entry.ReferenceData, entry.ReferenceType);
                return newValue;
            }
            else
            {
                return null;
            }
        }

        private void PasteEntry(SerializedProperty targetProperty, CopySerializeReferenceEntry entry)
        {
            var newValue = GetManagedReferenceValue(entry);
            targetProperty.managedReferenceValue = newValue;
        }

        public bool IsVisible(SerializedProperty property)
        {
            return PropertyUtility.IsSerializeReferenceProperty(property);
        }

        public bool IsEnabled(SerializedProperty property)
        {
            return IsOperationSupported(property, CopySerializeReferenceOperation.Cache);
        }

        public void Perform(SerializedProperty property)
        {
            var cache = CopySerializeReferenceOperation.Cache;
            var entries = cache.Entries;
            if (!IsAssignmentValid(property, entries))
            {
                ToolboxEditorLog.LogWarning("Cannot perform paste operation, types are mismatched.");
                return;
            }

            var targetProperty = property.Copy();
            try
            {
                targetProperty.serializedObject.Update();
                if (targetProperty.isArray)
                {
                    var arraySize = entries.Count;
                    targetProperty.arraySize = arraySize;
                    for (var i = 0; i < arraySize; i++)
                    {
                        var entry = entries[i];
                        var childProperty = targetProperty.GetArrayElementAtIndex(i);
                        PasteEntry(childProperty, entry);
                    }
                }
                else
                {
                    var entry = entries[0];
                    PasteEntry(targetProperty, entry);
                }

                targetProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (Exception)
            { }
        }

        public GUIContent Label => new GUIContent("Paste Serialized References");
    }
}
#endif