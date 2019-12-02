using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        private static bool IsSerializableArrayElement(SerializedProperty property)
        {
            return property.propertyPath.EndsWith("]");
        }

        /// <summary>
        /// Checks if a provided property is an array element, it uses associated fieldInfo to speed up inference.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static bool IsSerializableArrayElement(SerializedProperty property, FieldInfo fieldInfo)
        {
            return !property.isArray && IsSerializableArrayField(fieldInfo);
        }

        /// <summary>
        /// Returns provided element's index in serialized array.
        /// Method does not validate if <see cref="SerializedProperty"/> is truly an array element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static int GetPropertyElementIndex(SerializedProperty element)
        {
            const int indexPosition = 2;
            var indexChar = element.propertyPath[element.propertyPath.Length - indexPosition];
            return indexChar - '0';
        }

        private static string[] GetPropertyFieldTree(SerializedProperty property)
        {
            //unfortunately, we have to remove hard coded array properties since it's useless data
            return property.propertyPath.Replace("Array.data[", "[").Split('.').Where(field => field[0] != '[').ToArray();            
        }


        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute
        {
            return ReflectionUtility.GetField(GetTargetObject(property), property.name).GetCustomAttribute<T>(true);
        }

        public static T[] GetAttributes<T>(this SerializedProperty property) where T : Attribute
        {
            return (T[])ReflectionUtility.GetField(GetTargetObject(property), property.name).GetCustomAttributes(typeof(T), true);
        }

        public static Object GetTargetObject(this SerializedProperty property)
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
            var elements = element.propertyPath.Replace("Array.data[", "[").Split('.');

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

        internal static object GetDeclaringObject(this SerializedProperty property)
        {
            return GetDeclaringObject(property, property.serializedObject.targetObject);
        }

        internal static object GetDeclaringObject(this SerializedProperty property, Object target)
        {
            const BindingFlags propertyBindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var fields = GetPropertyFieldTree(property);
            var targetObject = target as object;
 
            if (fields.Length > 1)
            {
                var targetFieldInfo = target.GetType().GetField(fields[0], propertyBindings);

                targetObject = targetFieldInfo.GetValue(target);

                for (var i = 1; i < fields.Length - 1; i++)
                {
                    targetFieldInfo = targetObject.GetType().GetField(fields[i], propertyBindings);
                    targetObject = targetFieldInfo.GetValue(targetObject);
                }
            }

            return targetObject;
        }

        /// <summary>
        /// Returns proper <see cref="FieldInfo"/> value for this property, even if property is an array element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        /// <returns></returns>
        internal static object GetProperValue(this SerializedProperty property, FieldInfo fieldInfo)
        {
            return GetProperValue(property, fieldInfo, property.serializedObject.targetObject);
        }

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
        /// Sets value for property's field info in proper way. It does not matter if property is array element or single on.
        /// Method handles OnValidate call and multiple target objects but it's quite slow.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        internal static void SetProperValue(this SerializedProperty property, FieldInfo fieldInfo, object value)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var targets = property.serializedObject.targetObjects;
            var isElement = IsSerializableArrayElement(property, fieldInfo);

            for (var i = 0; i < targets.Length; i++)
            {
                var targetObject = targets[i];
                var target = property.GetDeclaringObject(targetObject);

                //record undo action, it will mark serialized component as dirty
                Undo.RecordObject(targetObject, "Set " + fieldInfo.Name);

                //handle situation when property is an array element
                if (isElement)
                {
                    var index = GetPropertyElementIndex(property);
                    var list = fieldInfo.GetValue(target) as IList;

                    list[index] = value;
                    fieldInfo.SetValue(target, list);
                }           
                //return fieldInfo value based on property's target object
                else
                {
                    fieldInfo.SetValue(target, value);
                }

                //simulate OnValidate call since we changed only fieldInfo's value
                ComponentUtility.SimulateOnValidate(targetObject);
            }    
        }

        /// <summary>
        /// Returns proper <see cref="Type"/> for this property, even if property is an array element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldInfo">FieldInfo associated to provided property.</param>
        /// <returns></returns>
        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo)
        {
            return GetProperType(property, fieldInfo, property.serializedObject.targetObject);
        }

        internal static Type GetProperType(this SerializedProperty property, FieldInfo fieldInfo, object declaringObject)
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

                return list[0].GetType();
            }
            //return fieldInfo value based on property's target object
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