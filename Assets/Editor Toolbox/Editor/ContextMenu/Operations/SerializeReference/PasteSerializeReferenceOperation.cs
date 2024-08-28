using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class PasteSerializeReferenceOperation : IContextMenuOperation
    {
        private bool IsAssignmentValid(SerializedProperty property, object newValue)
        {
#if UNITY_2021_3_OR_NEWER
            if (newValue == null)
            {
                return true;
            }

            if (!TypeUtility.TryGetTypeFromManagedReferenceFullTypeName(property.managedReferenceFieldTypename, out var referenceType))
            {
                return true;
            }

            var newValueType = newValue.GetType();
            if (TypeUtility.IsTypeAssignableFrom(referenceType, newValueType))
            {
                return true;
            }
#endif
            return false;
        }

        private object GetCachedManagedReferenceValue()
        {
            var cachedData = CopySerializeReferenceOperation.Cache;
            if (cachedData.ReferenceType != null)
            {
                var newValue = JsonUtility.FromJson(cachedData.Data, cachedData.ReferenceType);
                return newValue;
            }
            else
            {
                return null;
            }
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
            return CopySerializeReferenceOperation.Cache != null;
        }

        public void Perform(SerializedProperty property)
        {
#if UNITY_2019_3_OR_NEWER
            var targetProperty = property.Copy();
            try
            {
                var newValue = GetCachedManagedReferenceValue();
                if (!IsAssignmentValid(targetProperty, newValue))
                {
                    ToolboxEditorLog.LogWarning("Cannot perform paste operation, types are mismatched.");
                    return;
                }

                targetProperty.serializedObject.Update();
                targetProperty.managedReferenceValue = newValue;
                targetProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (Exception)
            { }
#endif
        }

        public GUIContent Label => new GUIContent("Paste Serialize Reference");
    }
}