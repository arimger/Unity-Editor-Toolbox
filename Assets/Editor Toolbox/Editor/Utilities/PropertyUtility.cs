using System;
using System.Reflection;

using UnityEditor;

namespace Toolbox.Editor
{
    public static class PropertyUtility
    {
        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute
        {
            T[] attributes = GetAttributes<T>(property);
            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(this SerializedProperty property) where T : Attribute
        {
            FieldInfo fieldInfo = ReflectionUtility.GetField(GetTargetObject(property), property.name);

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static UnityEngine.Object GetTargetObject(this SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }

        public static SerializedProperty GetArrayProperty(this SerializedProperty element)
        {
            var path = element.propertyPath.Replace("Array.data[", "[");
            var elements = path.Split('.');

            if (!elements[elements.Length - 1].Contains("[")) return null;

            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[i].Contains("[")) continue;
                return element.GetSibiling(elements[i]);
            }

            return null;
        }

        public static SerializedProperty GetSibiling(this SerializedProperty property, string propertyPath)
        {
            return property.depth == 0 || property.GetParent() == null
                ? property.serializedObject.FindProperty(propertyPath)
                : property.GetParent().FindPropertyRelative(propertyPath);
        }

        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            if (property.depth == 0) return null;

            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            SerializedProperty parent = null;

            for (int i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];
                var index = -1;
                if (element.Contains("["))
                {
                    index = Convert.ToInt32(element
                        .Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                }

                parent = i == 0 ? property.serializedObject.FindProperty(element) : parent.FindPropertyRelative(element);

                if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
            }

            return parent;
        }
    }
}