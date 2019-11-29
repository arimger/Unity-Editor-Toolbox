using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public static class ToolboxLogUtility
    {
        private static string GetPropertyLocation(SerializedProperty property)
        {
            return property.name + " property in " + property.serializedObject.targetObject;
        }


        public static void LogWrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, Type targetType)
        {
            LogWrongAttributeUsageWarning(property, attribute, targetType.ToString());
        }

        public static void LogWrongAttributeUsageWarning(SerializedProperty property, Attribute attribute, string targetType)
        {
            Debug.LogWarning(GetPropertyLocation(property) + " - " + attribute.GetType() + " can be used only on " + targetType + " type properties. " +
                             "Attribute will be ignored.");
        }

        public static void LogPropertyLocation(SerializedProperty property)
        {
            Debug.Log(GetPropertyLocation(property));
        }
    }
}