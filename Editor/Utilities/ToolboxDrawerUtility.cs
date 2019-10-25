using System;
using System.Reflection;

using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    [InitializeOnLoad]
    public static class ToolboxDrawerUtility
    {
        static ToolboxDrawerUtility()
        {
            propertyUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");

            //NOTE: in Unity 2019.3 it should be GetDrawerTypeForPropertyAndType
            getDrawerTypeForTypeMethod = propertyUtilityType.GetMethod("GetDrawerTypeForType", BindingFlags.NonPublic | BindingFlags.Static);
            getFieldInfoForPropertyMethod = propertyUtilityType.GetMethod("GetFieldInfoFromProperty", BindingFlags.NonPublic | BindingFlags.Static);
        }


        private static readonly Type propertyUtilityType;

        private static readonly MethodInfo getDrawerTypeForTypeMethod;
        private static readonly MethodInfo getFieldInfoForPropertyMethod;


        internal static ToolboxAreaDrawerBase GetAreaDrawer<T>(T attribute) where T : UnityEngine.ToolboxAreaAttribute
        {
            return ToolboxEditorUtility.GetAreaDrawer(attribute);
        }

        internal static ToolboxPropertyDrawerBase GetPropertyDrawer<T>(T attribute) where T : UnityEngine.ToolboxPropertyAttribute
        {
            return ToolboxEditorUtility.GetPropertyDrawer(attribute);
        }

        internal static ToolboxConditionDrawerBase GetConditionDrawer<T>(T attribute) where T : UnityEngine.ToolboxConditionAttribute
        {
            return ToolboxEditorUtility.GetConditionDrawer(attribute);
        }


        /// <summary>
        /// Check if for provided property(and property type) exist any custom drawer.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static bool PropertyHasCustomDrawer(SerializedProperty property, Type propertyType)
        {
            var parameters = new object[] { propertyType };
            var result = getDrawerTypeForTypeMethod.Invoke(null, parameters) as Type;
            return result != null && typeof(PropertyDrawer).IsAssignableFrom(result);
        }

        /// <summary>
        /// Get <see cref="FieldInfo"/> object associated to provided property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        internal static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type propertyType)
        {
            var parameters = new object[] { property, null };
            var result = getFieldInfoForPropertyMethod.Invoke(null, parameters);
            propertyType = parameters[1] as Type;
            return result as FieldInfo;
        }


        internal static bool ToolboxDrawersAllowed => ToolboxEditorUtility.ToolboxDrawersAllowed;
    }
}