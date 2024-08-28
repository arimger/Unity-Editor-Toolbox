using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.ContextMenu.Operations
{
    internal class DuplicateSerializeReferenceArrayElementOperation : IContextMenuOperation
    {
        public bool IsVisible(SerializedProperty property)
        {
#if UNITY_2021_3_OR_NEWER
            return property != null && property.propertyType == SerializedPropertyType.ManagedReference &&
                PropertyUtility.IsSerializableArrayElement(property);
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
            var sourceProperty = property.Copy();
            sourceProperty.serializedObject.Update();
            var sourceValue = sourceProperty.managedReferenceValue;

            var arrayProperty = PropertyUtility.GetArray(sourceProperty);
            var newElementIndex = arrayProperty.arraySize;
            arrayProperty.arraySize = newElementIndex + 1;
            //NOTE: there will be null by default anyway
            if (sourceValue != null)
            {
                var targetData = JsonUtility.ToJson(sourceValue);
                var targetValue = JsonUtility.FromJson(targetData, sourceValue.GetType());
                var targetProperty = arrayProperty.GetArrayElementAtIndex(newElementIndex);
                targetProperty.managedReferenceValue = targetValue;
            }

            sourceProperty.serializedObject.ApplyModifiedProperties();
#endif
        }

        public GUIContent Label => new GUIContent("Duplicate Serialize Reference Array Element");
    }
}