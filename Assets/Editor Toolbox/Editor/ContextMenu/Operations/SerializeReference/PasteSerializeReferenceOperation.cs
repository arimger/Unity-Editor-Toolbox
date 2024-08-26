using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu.Operations
{

    internal class PasteSerializeReferenceOperation : IContextMenuOperation
    {
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
                targetProperty.serializedObject.Update();
                targetProperty.managedReferenceValue = GetCachedManagedReferenceValue();
                targetProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (Exception)
            { }
#endif
        }

        public GUIContent Label => new GUIContent("Paste Serialize Reference");
    }
}