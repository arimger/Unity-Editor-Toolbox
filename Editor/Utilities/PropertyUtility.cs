using System;
using System.Collections;
using System.Reflection;

using UnityEditor;

namespace Toolbox.Editor
{
    public static class PropertyUtility
    {
        [InitializeOnLoadMethod]
        private static void SetUpReflectionMethods()
        {
            builtInPropertyUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");

            //TODO: handle this case:
            //NOTE: in Unity 2019.3 it should be GetDrawerTypeForPropertyAndType
            getDrawerTypeForTypeMethod = builtInPropertyUtilityType.GetMethod("GetDrawerTypeForType", 
                BindingFlags.NonPublic | BindingFlags.Static);
            getFieldInfoForPropertyMethod = builtInPropertyUtilityType.GetMethod("GetFieldInfoFromProperty",
                BindingFlags.NonPublic | BindingFlags.Static);
        }


        /// <summary>
        /// Native utility class used to handle <see cref="SerializedProperty"/> data.
        /// </summary>
        private static Type builtInPropertyUtilityType;

        private static MethodInfo getDrawerTypeForTypeMethod;
        private static MethodInfo getFieldInfoForPropertyMethod;


        private static bool IsBuiltInNumericProperty(SerializedProperty property)
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

        private static bool IsSerializableArrayField(FieldInfo fieldInfo)
        {
            return typeof(IList).IsAssignableFrom(fieldInfo.FieldType);
        }

        private static int GetPropertyElementIndex(SerializedProperty element)
        {
            const int indexPosition = 2;
            var indexChar = element.propertyPath[element.propertyPath.Length - indexPosition];
            return indexChar - '0';
        }


        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute
        {
            return ReflectionUtility.GetField(GetTargetObject(property), property.name).GetCustomAttribute<T>(true);
        }

        public static T[] GetAttributes<T>(this SerializedProperty property) where T : Attribute
        {
            return (T[])ReflectionUtility.GetField(GetTargetObject(property), property.name).GetCustomAttributes(typeof(T), true);
        }

        public static UnityEngine.Object GetTargetObject(this SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }

        /// <summary>
        /// Gets array property from its child element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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


        internal static FieldInfo GetFieldInfo(this SerializedProperty property, out Type propertyType)
        {
            var parameters = new object[] { property, null };
            var result = getFieldInfoForPropertyMethod.Invoke(null, parameters);
            propertyType = parameters[1] as Type;
            return result as FieldInfo;
        }

        /// <summary>
        /// Creates unique key for this property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static string GetPropertyKey(this SerializedProperty property)
        {
            return property.serializedObject.GetHashCode() + "-" + property.propertyPath;
        }

        /// <summary>
        /// Returns proper <see cref="FieldInfo"/> value for this property, even if property is an array element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        internal static object GetProperValue(this SerializedProperty property, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (IsSerializableArrayField(fieldInfo))
            {
                var index = GetPropertyElementIndex(property);
                var list = fieldInfo.GetValue(property.serializedObject.targetObject) as IList;

                return list[index];
            }
            else
            {
                return fieldInfo.GetValue(property.serializedObject.targetObject);
            }
        }

        /// <summary>
        /// Sets value for property's field info in proper way. It does not matter if property is array element or single on.
        /// Method handles OnValidate call and multiple target objects but it's quite slow.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo"></param>
        internal static void SetProperValue(this SerializedProperty property, FieldInfo fieldInfo, object value)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var targets = property.serializedObject.targetObjects;

            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];

                if (IsSerializableArrayField(fieldInfo))
                {
                    var index = GetPropertyElementIndex(property);
                    var list = fieldInfo.GetValue(target) as IList;

                    list[index] = value;
                    fieldInfo.SetValue(target, list);
                }
                else
                {
                    fieldInfo.SetValue(target, value);
                }

                ComponentUtility.SimulateOnValidate(target);
            }    
        }

        /// <summary>
        /// Returns proper <see cref="Type"/> for this property, even if property is an array element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (IsSerializableArrayField(fieldInfo))
            {
                var index = GetPropertyElementIndex(property);
                var list = fieldInfo.GetValue(property.serializedObject.targetObject) as IList;
        
                return list[0].GetType();
            }
            else
            {
                return fieldInfo.FieldType;
            }
        }

        internal static bool HasCustomDrawer(this SerializedProperty property, Type drawerType)
        {
            if (IsBuiltInNumericProperty(property))
            {
                return true;
            }

            var parameters = new object[] { drawerType };
            var result = getDrawerTypeForTypeMethod.Invoke(null, parameters) as Type;
            return result != null && typeof(PropertyDrawer).IsAssignableFrom(result);
        }
    }
}