using System;
using System.Text;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    internal static class ToolboxEditorLog
    {
        private const string tag = "Editor Toolbox";
        private const string format = "[{0}] {1}";


        private static string GetPropertySceneLocation(SerializedProperty property)
        {
            return string.Format("{0} property in {1}", property.name, property.serializedObject.targetObject);
        }

        private static string GetMemberNotFoundMessage(Type classType, string memberName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(classType != null ? string.Format("{0}: ", classType) : string.Empty);
            memberName = string.IsNullOrEmpty(memberName) ? "<Empty>" : memberName;
            stringBuilder.Append(string.Format("Member ({0}) not found.", memberName));
            return stringBuilder.ToString();
        }


        internal static void AttributeUsageWarning(Attribute attribute, string message)
        {
            AttributeUsageWarning(attribute.GetType(), message);
        }

        internal static void AttributeUsageWarning(Type attributeType, string message)
        {
            LogWarning(attributeType.Name + ": " + message);
        }

        internal static void AttributeUsageWarning(Attribute attribute, SerializedProperty property, string message)
        {
            AttributeUsageWarning(attribute.GetType(), property, message);
        }

        internal static void AttributeUsageWarning(Type attributeType, SerializedProperty property, string message)
        {
            LogWarning(attributeType.Name + ": " + GetPropertySceneLocation(property) + ": " + message);
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property)
        {
            AttributeUsageWarning(attribute, property, "Assigned attribute cannot be used on this property.");
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, Type targetType)
        {
            WrongAttributeUsageWarning(attribute, property, targetType.ToString());
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, string targetType)
        {
            AttributeUsageWarning(attribute, property, "Assigned attribute can be used only on " + targetType + " type properties.");
        }

        internal static void AttributeNotSupportedWarning(Attribute attribute)
        {
            AttributeNotSupportedWarning(attribute.GetType());
        }

        internal static void AttributeNotSupportedWarning(Type attributeType)
        {
            LogWarning(attributeType + " is not supported. Assign it in the " + nameof(ToolboxEditorSettings) + ".");
        }

        internal static void PropertyNotFoundWarning(SerializedProperty property, string propertyName)
        {
            LogWarning(GetPropertySceneLocation(property) + ": Property " + propertyName + " not found.");
        }

        internal static void TypeNotSupportedWarning(SerializedProperty property, Type type)
        {
            TypeNotSupportedWarning(property, type.Name);
        }

        internal static void TypeNotSupportedWarning(SerializedProperty property, string type)
        {
            LogWarning(GetPropertySceneLocation(property) + ": " + type + " value type is not supported in comparison.");
        }

        internal static void PropertyLocation(SerializedProperty property)
        {
            LogInfo(GetPropertySceneLocation(property));
        }

        internal static void MemberNotFoundWarning(Attribute attribute, Type classType, string memberName)
        {
            AttributeUsageWarning(attribute, GetMemberNotFoundMessage(classType, memberName));
        }

        internal static void MemberNotFoundWarning(Attribute attribute, SerializedProperty property, string memberName)
        {
            AttributeUsageWarning(attribute, property, GetMemberNotFoundMessage(null, memberName));
        }

        internal static void MemberNotFoundWarning(Attribute attribute, SerializedProperty property, Type classType, string memberName)
        {
            AttributeUsageWarning(attribute, property, GetMemberNotFoundMessage(classType, memberName));
        }

        internal static void MemberNotFoundWarning(Type classType, string memberName)
        {
            LogWarning(GetMemberNotFoundMessage(classType, memberName));
        }

        internal static void PrefabExpectedWarning()
        {
            PrefabExpectedWarning(null);
        }

        internal static void PrefabExpectedWarning(Object referenceObject)
        {
            var name = referenceObject ? referenceObject.name : "object";
            LogWarning(name + " should be a prefab.");
        }

        internal static void KitInitializationMessage()
        {
            LogWarning("Settings initialization needed. Go to <b>Edit/Project Settings.../Editor Toolbox</b> and find an existing Settings file ('Refresh') or create new.");
        }

        internal static void LogWarning(string message)
        {
            LogMessage(message, LogType.Warning);
        }

        internal static void LogError(string message)
        {
            LogMessage(message, LogType.Error);
        }

        internal static void LogInfo(string message)
        {
            LogMessage(message, LogType.Log);
        }

        internal static void LogMessage(string message, LogType logType)
        {
#if UNITY_2019_1_OR_NEWER
            Debug.LogFormat(logType, LogOption.NoStacktrace, null, format, tag, message);
#else
            Debug.unityLogger.LogFormat(logType, format, tag, message);
#endif
        }
    }
}