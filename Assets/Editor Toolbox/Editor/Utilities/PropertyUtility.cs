using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEditor;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    public static partial class PropertyUtility
    {
        /// <summary>
        /// Creates and returns unique (hash based) key for this property.
        /// </summary>
        internal static string GetPropertyHashKey(this SerializedProperty property)
        {
            var hash = property.serializedObject.GetHashCode();
            return string.Format("{0}.{1}", hash, property.propertyPath);
        }

        /// <summary>
        /// Creates and returns unique (type based) key for this property.
        /// </summary>
        internal static string GetPropertyTypeKey(this SerializedProperty property)
        {
            var type = property.serializedObject.targetObject.GetType();
            return string.Format("{0}.{1}", type, property.propertyPath);
        }

        /// <summary>
        /// Returns <see cref="object"/> which truly declares this property.
        /// </summary>
        internal static object GetDeclaringObject(this SerializedProperty property)
        {
            return GetDeclaringObject(property, property.serializedObject.targetObject);
        }

        /// <summary>
        /// Returns <see cref="object"/> which truly declares this property.
        /// </summary>
        internal static object GetDeclaringObject(this SerializedProperty property, Object target)
        {
            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var members = GetPropertyFieldTree(property);
            var instance = target as object;

            if (members.Length > 1)
            {
                var fieldInfo = target.GetType().GetField(members[0], bindings);
                instance = fieldInfo.GetValue(target);

                for (var i = 1; i < members.Length - 1; i++)
                {
                    fieldInfo = instance.GetType().GetField(members[i], bindings);
                    instance = fieldInfo.GetValue(instance);
                }
            }

            return instance;
        }

        /// <summary>
        /// Returns proper <see cref="FieldInfo"/> value for this property, even if the property is an array element.
        /// </summary>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static object GetProperValue(this SerializedProperty property, FieldInfo fieldInfo)
        {
            return GetProperValue(property, fieldInfo, property.serializedObject.targetObject);
        }

        /// <summary>
        /// Returns proper <see cref="FieldInfo"/> value for this property, even if the property is an array element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static object GetProperValue(this SerializedProperty property, FieldInfo fieldInfo, object declaringObject)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            //handle situation when property is an array element
            if (IsSerializableArrayElement(property, fieldInfo))
            {
                var index = GetPropertyElementIndex(property);
                var list = fieldInfo.GetValue(declaringObject) as IList;

                return list[index];
            }
            //return fieldInfo value based on property's target object
            else
            {
                return fieldInfo.GetValue(declaringObject);
            }
        }

        /// <summary>
        /// Sets value for property's field info in proper way.
        /// It does not matter if property is an array element or single.
        /// Method handles OnValidate call and multiple target objects but it's quite slow.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static void SetProperValue(this SerializedProperty property, FieldInfo fieldInfo, object value)
        {
            SetProperValue(property, fieldInfo, value, true);
        }

        /// <summary>
        /// Sets value for property's field info in proper way.
        /// It does not matter if property is an array element or single.
        /// Method handles OnValidate call and multiple target objects but it's quite slow.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static void SetProperValue(this SerializedProperty property, FieldInfo fieldInfo, object value, bool callOnValidate)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var targets = property.serializedObject.targetObjects;
            var element = IsSerializableArrayElement(property, fieldInfo);

            for (var i = 0; i < targets.Length; i++)
            {
                var targetObject = targets[i];
                var targetParent = property.GetDeclaringObject(targetObject);

                //record undo action, it will mark serialized component as dirty
                Undo.RecordObject(targetObject, "Set " + fieldInfo.Name);

                //handle situation when property is an array element
                if (element)
                {
                    var index = GetPropertyElementIndex(property);
                    var list = fieldInfo.GetValue(targetParent) as IList;
                    list[index] = value;
                    fieldInfo.SetValue(targetParent, list);
                }
                //return fieldInfo value based on property's target object
                else
                {
                    fieldInfo.SetValue(targetParent, value);
                }

                if (callOnValidate)
                {
                    //simulate OnValidate call since we changed fieldInfo's value
                    InspectorUtility.SimulateOnValidate(targetObject);
                }
            }
        }

        /// <summary>
        /// Returns proper <see cref="Type"/> for this property, even if the property is an array element.
        /// </summary>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo)
        {
            return GetProperType(property, fieldInfo, property.serializedObject.targetObject);
        }

        /// <summary>
        /// Returns proper <see cref="Type"/> for this property, even if the property is an array element.
        /// </summary>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo, object declaringObject)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            //handle situation when property is an array element
            if (IsSerializableArrayElement(property, fieldInfo))
            {
                var list = fieldInfo.GetValue(declaringObject) as IList;
                return list[0].GetType();
            }
            //return fieldInfo value based on property's target object
            else
            {
                return fieldInfo.FieldType;
            }
        }

        internal static Type GetScriptTypeFromProperty(SerializedProperty property)
        {
            var scriptProperty = property.serializedObject.FindProperty(Defaults.scriptPropertyName);
            if (scriptProperty == null)
            {
                return null;
            }

            var scriptInstance = scriptProperty.objectReferenceValue as MonoScript;
            if (scriptInstance == null)
            {
                return null;
            }

            return scriptInstance.GetClass();
        }


        internal static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            return GetFieldInfo(property, out _);
        }

        internal static FieldInfo GetFieldInfo(this SerializedProperty property, out Type propertyType)
        {
            return GetFieldInfoFromProperty(property, out propertyType);
        }

        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            var classType = GetScriptTypeFromProperty(property);
            if (classType == null)
            {
                type = null;
                return null;
            }

            return GetFieldInfoFromProperty(classType, property.propertyPath, out type);
        }

        public static FieldInfo GetFieldInfoFromProperty(Type host, string fieldPath, out Type type)
        {
            FieldInfo field = null;
            type = host;
            var parts = fieldPath.Split('.');

            for (var i = 0; i < parts.Length; i++)
            {
                var member = parts[i];
                if (i < parts.Length - 1 && member == "Array" && parts[i + 1].StartsWith("data["))
                {
                    if (IsSerializableArrayType(type))
                    {
                        type = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
                    }

                    i++;
                    continue;
                }

                FieldInfo foundField = null;
                for (var currentType = type; foundField == null && currentType != null; currentType = currentType.BaseType)
                {
                    foundField = currentType.GetField(member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (foundField == null)
                {
                    type = null;
                    return null;
                }

                field = foundField;
                type = field.FieldType;
            }

            return field;
        }


        internal static class Defaults
        {
            internal static readonly string scriptPropertyName = "m_Script";
        }
    }

    public static partial class PropertyUtility
    {
        public static SerializedProperty GetSibiling(this SerializedProperty property, string propertyPath)
        {
            var propertyParent = property.GetParent();
            return propertyParent == null
                ? property.serializedObject.FindProperty(propertyPath)
                : propertyParent.FindPropertyRelative(propertyPath);
        }

        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            if (property.depth == 0)
            {
                return null;
            }

            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            SerializedProperty parent = null;

            for (var i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];
                var index = -1;
                if (element.Contains("["))
                {
                    index = Convert.ToInt32(element
                        .Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "").Replace("]", ""));
                    element = element
                        .Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                }

                parent = i == 0 ? property.serializedObject.FindProperty(element) : parent.FindPropertyRelative(element);

                if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
            }

            return parent;
        }

        public static SerializedProperty GetArray(this SerializedProperty element)
        {
            var elements = element.propertyPath.Replace("Array.data[", "[").Split('.');
            if (!elements[elements.Length - 1].Contains("["))
            {
                return null;
            }

            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[i].Contains("[")) continue;
                return element.GetSibiling(elements[i]);
            }

            return null;
        }

        public static SerializedProperty GetSize(this SerializedProperty array)
        {
            return array.FindPropertyRelative("Array.size");
        }


        public static T GetAttribute<T>(SerializedProperty property) where T : Attribute
        {
            return GetAttribute<T>(property, GetFieldInfoFromProperty(property, out var type));
        }

        public static T GetAttribute<T>(SerializedProperty property, FieldInfo fieldInfo) where T : Attribute
        {
            return fieldInfo.GetCustomAttribute<T>(true);
        }

        public static T[] GetAttributes<T>(SerializedProperty property) where T : Attribute
        {
            return GetAttributes<T>(property, GetFieldInfoFromProperty(property, out var type));
        }

        public static T[] GetAttributes<T>(SerializedProperty property, FieldInfo fieldInfo) where T : Attribute
        {
            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }


        internal static bool IsLastSerializedProperty(SerializedProperty property)
        {
            return !property.Copy().NextVisible(false);
        }

        internal static bool IsBuiltInNumericProperty(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector4:
                    return true;
            }

            return false;
        }

        internal static bool HasVisibleChildrenFields(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                    return false;
            }

            return property.hasVisibleChildren;
        }

        internal static bool IsSerializableArrayType(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        internal static bool IsSerializableArrayField(FieldInfo fieldInfo)
        {
            return IsSerializableArrayType(fieldInfo.FieldType);
        }

        internal static bool IsSerializableArrayElement(SerializedProperty property)
        {
            return property.propertyPath.EndsWith("]");
        }

        internal static bool IsSerializableArrayElement(SerializedProperty property, Type type)
        {
            return !property.isArray && IsSerializableArrayType(type);
        }

        internal static bool IsSerializableArrayElement(SerializedProperty property, FieldInfo fieldInfo)
        {
            return !property.isArray && IsSerializableArrayType(fieldInfo.FieldType);
        }

        internal static bool IsDefaultScriptProperty(SerializedProperty property)
        {
            return IsDefaultScriptPropertyByPath(property.propertyPath);
        }

        internal static bool IsDefaultScriptPropertyByPath(string propertyPath)
        {
            return propertyPath == "m_Script";
        }

        internal static bool IsDefaultScriptPropertyByType(string propertyType)
        {
            return propertyType == "PPtr<MonoScript>";
        }

        internal static int GetPropertyElementIndex(SerializedProperty element)
        {
            if (!IsSerializableArrayElement(element))
            {
                return -1;
            }

            var indexString = string.Empty;
            var propertyPath = element.propertyPath;
            for (var i = propertyPath.Length - 2; i >= 0; i--)
            {
                var character = propertyPath[i];
                if (character.Equals('['))
                {
                    break;
                }

                indexString = character + indexString;
            }
;
            return int.Parse(indexString);
        }

        internal static string[] GetPropertyFieldTree(SerializedProperty property)
        {
            //unfortunately, we have to remove hard coded array properties since it's useless data
            return property.propertyPath.Replace("Array.data[", "[").Split('.').Where(field => field[0] != '[').ToArray();
        }
    }
}