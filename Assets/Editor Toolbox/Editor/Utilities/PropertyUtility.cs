using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEditor;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("Toolbox.Editor.Tests")]

namespace Toolbox.Editor
{
    public static partial class PropertyUtility
    {
        /// <summary>
        /// Indicates if the property has all changes applied and can be safely used for reflection-based features.
        /// </summary>
        internal static bool HasModifedProperties(this SerializedProperty property)
        {
            return property.serializedObject.hasModifiedProperties;
        }

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
        internal static object GetDeclaringObject(this SerializedProperty property, bool ignoreArrays)
        {
            return GetDeclaringObject(property, property.serializedObject.targetObject, ignoreArrays);
        }

        /// <summary>
        /// Returns <see cref="object"/> which truly declares this property.
        /// </summary>
        internal static object GetDeclaringObject(this SerializedProperty property, Object target)
        {
            return GetDeclaringObject(property, target, true);
        }

        /// <summary>
        /// Returns <see cref="object"/> which truly declares this property.
        /// </summary>
        internal static object GetDeclaringObject(this SerializedProperty property, Object target, bool ignoreArrays)
        {
            EnsureReflectionSafeness(property);

            var reference = target as object;
            var validReference = reference;
            var members = GetPropertyFieldTree(property);
            if (members.Length > 1)
            {
                for (var i = 0; i < members.Length - 1; i++)
                {
                    var treeField = members[i];
                    reference = GetTreePathReference(treeField, reference);
                    if (ignoreArrays && IsSerializableArrayType(reference))
                    {
                        continue;
                    }

                    validReference = reference;
                }
            }

            return validReference;
        }

        internal static object GetTreePathReference(string treeField, object treeParent)
        {
            if (IsSerializableArrayElement(treeField, out var index))
            {
                if (treeParent is IList list)
                {
                    return list[index];
                }

                ToolboxEditorLog.LogError("Cannot parse array element properly.");
            }

            var fieldType = treeParent.GetType();
            var fieldInfo = fieldType.GetField(treeField,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fieldInfo.GetValue(treeParent);
        }

        /// <summary>
        /// Returns proper <see cref="FieldInfo"/> value for this property, even if the property is an array element.
        /// </summary>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static object GetProperValue(this SerializedProperty property, FieldInfo fieldInfo)
        {
            var declaringObject = property.GetDeclaringObject();
            return GetProperValue(property, fieldInfo, declaringObject);
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

            var targetObjects = property.serializedObject.targetObjects;
            var isArrayElement = IsSerializableArrayElement(property, fieldInfo);

            for (var i = 0; i < targetObjects.Length; i++)
            {
                var targetObject = targetObjects[i];
                var targetParent = property.GetDeclaringObject(targetObject);

                Undo.RecordObject(targetObject, string.Format("Set {0}", fieldInfo.Name));
                //handle situation when property is an array element
                if (isArrayElement)
                {
                    var index = GetPropertyElementIndex(property);
                    var list = fieldInfo.GetValue(targetParent) as IList;
                    list[index] = value;
                    fieldInfo.SetValue(targetParent, list);
                }
                else
                {
                    fieldInfo.SetValue(targetParent, value);
                }

                ForceOnValidateBroadcast(targetObject, callOnValidate);
                ForceTargetSerialization(targetObject);
            }
        }

        /// <summary>
        /// Returns proper <see cref="Type"/> for this property, even if the property is an array element.
        /// </summary>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var fieldType = fieldInfo.FieldType;
            //handle situation when property is an array element
            if (IsSerializableArrayElement(property, fieldInfo))
            {
                return fieldType.IsGenericType
                    ? fieldType.GetGenericArguments()[0]
                    : fieldType.GetElementType();
            }
            //return fieldInfo type based on property's target object
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

        internal static FieldInfo GetFieldInfo(this SerializedProperty property, out Type propertyType, Object target)
        {
            return GetFieldInfoFromProperty(target.GetType(), property.propertyPath, out propertyType);
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

            var members = GetPropertyFieldTree(fieldPath, false);
            for (var i = 0; i < members.Length; i++)
            {
                var member = members[i];
                if (IsSerializableArrayElement(member, out _))
                {
                    if (IsSerializableArrayType(type))
                    {
                        type = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
                    }

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
        public static SerializedProperty GetSibling(this SerializedProperty property, string propertyPath)
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

            SerializedProperty parent = null;
            var members = GetPropertyFieldTree(property);
            if (members.Length > 1)
            {
                parent = property.serializedObject.FindProperty(members[0]);
                for (var i = 1; i < members.Length - 1; i++)
                {
                    var fieldName = members[i];
                    parent = IsSerializableArrayElement(fieldName, out var index)
                        ? parent.GetArrayElementAtIndex(index)
                        : parent.FindPropertyRelative(fieldName);
                }
            }

            return parent;
        }

        public static SerializedProperty GetArray(this SerializedProperty element)
        {
            return element.GetParent();
        }

        public static SerializedProperty GetSize(this SerializedProperty array)
        {
            return array.FindPropertyRelative("Array.size");
        }


        public static T GetAttribute<T>(SerializedProperty property) where T : Attribute
        {
            return GetAttribute<T>(property, GetFieldInfoFromProperty(property, out _));
        }

        public static T GetAttribute<T>(SerializedProperty property, FieldInfo fieldInfo) where T : Attribute
        {
            return fieldInfo.GetCustomAttribute<T>(true);
        }

        public static T[] GetAttributes<T>(SerializedProperty property) where T : Attribute
        {
            return GetAttributes<T>(property, GetFieldInfoFromProperty(property, out _));
        }

        public static T[] GetAttributes<T>(SerializedProperty property, FieldInfo fieldInfo) where T : Attribute
        {
            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }


        internal static void EnsureReflectionSafeness(SerializedProperty property)
        {
            if (property.serializedObject.hasModifiedProperties)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        internal static void ForceTargetSerialization(Object targetObject)
        {
            if (AssetDatabase.Contains(targetObject))
            {
                EditorUtility.SetDirty(targetObject);
            }
        }

        internal static void ForceOnValidateBroadcast(Object targetObject, bool isForced)
        {
            if (isForced)
            {
                InspectorUtility.SimulateOnValidate(targetObject);
            }
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

        internal static bool IsSerializableArrayType(object target)
        {
            return IsSerializableArrayType(target.GetType());
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

        internal static bool IsSerializableArrayElement(string indexField, out int index)
        {
            if (indexField.StartsWith("["))
            {
                index = int.Parse(indexField.Substring(1, indexField.Length - 2));
                return true;
            }

            index = -1;
            return false;
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
            return GetPropertyFieldTree(property, false);
        }

        internal static string[] GetPropertyFieldTree(SerializedProperty property, bool ignoreArrayElements)
        {
            return GetPropertyFieldTree(property.propertyPath, ignoreArrayElements);
        }

        internal static string[] GetPropertyFieldTree(string propertyPath, bool ignoreArrayElements)
        {
            //unfortunately, we have to remove hard coded array properties since it's useless data
            return ignoreArrayElements
                ? propertyPath.Replace("Array.data[", "[").Split('.').Where(field => field[0] != '[').ToArray()
                : propertyPath.Replace("Array.data[", "[").Split('.');
        }
    }
}