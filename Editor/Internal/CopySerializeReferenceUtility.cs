using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    [InitializeOnLoad]
    public static class CopySerializeReferenceUtility
    {
        static CopySerializeReferenceUtility()
        {
            propertyToCopy = null;
            EditorApplication.contextualPropertyMenu -= OnContextMenuOpening;
            EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
        }

        private static SerializedProperty propertyToCopy;

        private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference || 
                !PropertyUtility.IsSerializableArrayElement(property))
            {
                return;
            }

            menu.AddItem(new GUIContent("Copy Reference"), false, () =>
            {
                propertyToCopy = property;
            });
            menu.AddItem(new GUIContent("Duplicate Reference"), false, () =>
            {
                var parent = property.GetParent();
                parent.arraySize++;
                parent.serializedObject.ApplyModifiedProperties();
                var newProperty = parent.GetArrayElementAtIndex(parent.arraySize - 1); 
            });
            if (propertyToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste Reference"), false, () =>
                {
                    property.serializedObject.Update();
                    //property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                    propertyToCopy = null;
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste Reference"));
            }
        }
    }
}
