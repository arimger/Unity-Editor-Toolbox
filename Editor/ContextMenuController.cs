using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    internal static class ContextMenuController
    {
        static ContextMenuController()
        {
            EditorApplication.contextualPropertyMenu -= OnContextMenuOpening;
            EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
        }

        private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property)
        {
            HandleSerializeReference(menu, property);
        }

        //TODO: dedicated class to handle it
        private static void HandleSerializeReference(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference ||
                !PropertyUtility.IsSerializableArrayElement(property))
            {
                return;
            }

            menu.AddItem(new GUIContent("Copy Reference"), false, () =>
            {
                //propertyToCopy = property;
            });
            //if (propertyToCopy != null)
            //{
            //    menu.AddItem(new GUIContent("Paste Reference"), false, () =>
            //    {
            //        property.serializedObject.Update();
            //        //property.managedReferenceValue = null;
            //        property.serializedObject.ApplyModifiedProperties();
            //        propertyToCopy = null;
            //    });
            //}
            //else
            {
                menu.AddDisabledItem(new GUIContent("Paste Reference"));
            }

            menu.AddItem(new GUIContent("Duplicate Reference"), false, () =>
            {
                var parent = property.GetParent();
                parent.arraySize++;
                parent.serializedObject.ApplyModifiedProperties();
                var newProperty = parent.GetArrayElementAtIndex(parent.arraySize - 1);
                //TODO: fill all serializable fields
            });
        }
    }
}