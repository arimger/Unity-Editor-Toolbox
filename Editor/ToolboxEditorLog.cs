using System;

using UnityEditor;
using UnityEngine;

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
            Debug.LogWarning(GetPropertyLocation(property) + " - " + attribute.GetType() + " cannot be used on this property.");
        }

        public static void WrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, Type targetType)
        {
            WrongAttributeUsageWarning(property, attribute, targetType.ToString());
        }

        public static void WrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, string targetType)
        {
            Debug.LogWarning(GetPropertyLocation(property) + " - " + attribute.GetType() + " can be used only on " + targetType + " type properties. " +
                             "Attribute will be ignored.");
        }

        public static void PropertyLocation(SerializedProperty property)
        {
            Debug.Log(GetPropertyLocation(property));
        }
    }
}