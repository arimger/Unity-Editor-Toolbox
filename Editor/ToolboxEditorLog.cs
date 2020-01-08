using System;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    public static class ToolboxEditorLog
    {
        private static string GetPropertyLocation(SerializedProperty property)
        {
            return property.name + " property in " + property.serializedObject.targetObject;
        }


        public static void WrongAttributeUsageWarning(SerializedProperty property, Attribute attribute)
        {
            Debug.LogWarning(GetPropertyLocation(property) + ": " + attribute.GetType() + " cannot be used on this property.");
        }

        public static void WrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, Type targetType)
        {
            WrongAttributeUsageWarning(property, attribute, targetType.ToString());
        }

        public static void WrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, string targetType)
        {
            Debug.LogWarning(GetPropertyLocation(property) + ": " + attribute.GetType() + " can be used only on " + targetType + " type properties. " +
                             "Attribute will be ignored.");
        }

        public static void PropertyNotFoundWarning(SerializedProperty property, string propertyName)
        {
            Debug.LogWarning(GetPropertyLocation(property) + ": Property " + propertyName + " not found.");
        }

        public static void TypeNotSupportedWarning(SerializedProperty property, Type type)
        {
            TypeNotSupportedWarning(property, type.Name);
        }

        public static void TypeNotSupportedWarning(SerializedProperty property, string type)
        {
            Debug.LogWarning(GetPropertyLocation(property) + ": " + type + " value type is not supported in comparison.");
        }

        public static void PropertyLocation(SerializedProperty property)
        {
            Debug.Log(GetPropertyLocation(property));
        }

        public static void KitInitializationWarning(string settingsTypeName)
        {
            var message = settingsTypeName +
                          " asset file not found. Cannot initialize Toolbox core functionalities. " +
                          "You can create new settings file using CreateAsset menu -> Create -> Toolbox Editor -> Settings.";
            Debug.LogWarning(message);
        }

        public static void AttributeNotSupportedWarning(Attribute attribute)
        {
            Debug.LogWarning(attribute.GetType() + " is not supported. Assign it in " + nameof(ToolboxEditorSettings) + ".");
        }

        public static void PrefabExpectedWarning()
        {
            PrefabExpectedWarning(null);
        }

        public static void PrefabExpectedWarning(Object referenceObject)
        {
            var name = referenceObject ? referenceObject.name : "object";
            Debug.LogWarning(name + " should be a prefab.");
        }

        public static void LogWarning(string message)
        {
            //Debug.LogWarning("Toolbox Editor Kit: " + message);
            throw new NotImplementedException();
        }
    }
}