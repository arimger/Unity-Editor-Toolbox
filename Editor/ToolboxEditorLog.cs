using System;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    internal static class ToolboxEditorLog
    {
        private static string GetPropertyLocation(SerializedProperty property)
        {
            return property.name + " property in " + property.serializedObject.targetObject;
        }


        public static void AttributeUsageWarning(Attribute attribute, string message)
        {
            LogWarning(attribute.GetType().Name + ": " + message);
        }

        public static void AttributeUsageWarning(Attribute attribute, SerializedProperty property, string message)
        {
            LogWarning(attribute.GetType().Name + ": " + GetPropertyLocation(property) + ": " + message);
        }

        public static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property)
        {
            AttributeUsageWarning(attribute, property, "Associated attribute cannot be used on this property.");
        }

        public static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, Type targetType)
        {
            WrongAttributeUsageWarning(attribute, property, targetType.ToString());
        }

        public static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, string targetType)
        {
            AttributeUsageWarning(attribute, property, "Associated attribute can be used only on " + targetType + " type properties.");
        }

        public static void AttributeNotSupportedWarning(Attribute attribute)
        {
            LogWarning(attribute.GetType() + " is not supported. Assign it in the " + nameof(ToolboxEditorSettings) + ".");
        }

        public static void PropertyNotFoundWarning(SerializedProperty property, string propertyName)
        {
            LogWarning(GetPropertyLocation(property) + ": Property " + propertyName + " not found.");
        }

        public static void TypeNotSupportedWarning(SerializedProperty property, Type type)
        {
            TypeNotSupportedWarning(property, type.Name);
        }

        public static void TypeNotSupportedWarning(SerializedProperty property, string type)
        {
            LogWarning(GetPropertyLocation(property) + ": " + type + " value type is not supported in comparison.");
        }

        public static void PropertyLocation(SerializedProperty property)
        {
            LogMessage(GetPropertyLocation(property));
        }

        public static void PrefabExpectedWarning()
        {
            PrefabExpectedWarning(null);
        }

        public static void PrefabExpectedWarning(Object referenceObject)
        {
            var name = referenceObject ? referenceObject.name : "object";
            LogWarning(name + " should be a prefab.");
        }

        public static void KitInitializationMessage()
        {
            LogMessage("Settings initialization needed. Go to Edit/Project Settings.../Editor Toolbox and pick an existing Settings file or create new. ");
        }


        public static void LogMessage(string message)
        {
            Debug.LogWarning("[Editor Toolbox] " + message);
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning("[Editor Toolbox] " + message);
        }


        public static void LogError(string message)
        {
            Debug.LogError("[Editor Toolbox] " + message);
        }     
    }
}