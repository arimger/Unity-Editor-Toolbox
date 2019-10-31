using System;
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


        private static Type builtInPropertyUtilityType;

        private static MethodInfo getDrawerTypeForTypeMethod;
        private static MethodInfo getFieldInfoForPropertyMethod;


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

        internal static string GetPropertyKey(this SerializedProperty property)
        {
            return property.serializedObject.GetHashCode() + "-" + property.propertyPath;
        }

        internal static bool HasCustomDrawer(this SerializedProperty property, Type drawerType)
        {
            //NOTE: property reference will be helpuf in future releases
            var parameters = new object[] { drawerType };
            var result = getDrawerTypeForTypeMethod.Invoke(null, parameters) as Type;
            return result != null && typeof(PropertyDrawer).IsAssignableFrom(result);
        }
    }
}