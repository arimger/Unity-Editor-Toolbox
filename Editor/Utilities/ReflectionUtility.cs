using System;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    internal static class ReflectionUtility
    {
        private static readonly Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

        public const BindingFlags allBindings = BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        internal static FieldInfo GetField(Type targetType, string fieldName)
        {
            return GetField(targetType, fieldName, allBindings);
        }

        internal static FieldInfo GetField(Type targetType, string fieldName, BindingFlags bindingFlags)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            var field = targetType.GetField(fieldName, bindingFlags);
            if (field == null)
            {
                //NOTE: if a method is not found and we searching for a private method we should look into parent classes
                Type baseType = targetType.BaseType;
                while (baseType != null)
                {
                    field = baseType.GetField(fieldName, bindingFlags);
                    if (field != null)
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return field;
        }

        /// <summary>
        /// Returns <see cref="MethodInfo"/> of the searched method within the Editor <see cref="Assembly"/>.
        /// </summary>
        internal static MethodInfo GetEditorMethod(string classType, string methodName, BindingFlags falgs)
        {
            return editorAssembly.GetType(classType).GetMethod(methodName, falgs);
        }

        internal static MethodInfo GetMethod(string methodName, SerializedObject serializedObject)
        {
            return GetMethod(methodName, serializedObject.targetObjects);
        }

        internal static MethodInfo GetMethod(string methodName, params object[] targetObjects)
        {
            return GetMethod(methodName, allBindings, targetObjects);
        }

        internal static MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, params object[] targetObjects)
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return null;
            }

            var targetType = targetObjects[0].GetType();
            return GetMethod(targetType, methodName, bindingFlags);
        }

        internal static MethodInfo GetMethod(Type targetType, string methodName)
        {
            return GetMethod(targetType, methodName, allBindings);
        }

        internal static MethodInfo GetMethod(Type targetType, string methodName, BindingFlags bindingFlags)
        {
            var methodInfo = targetType.GetMethod(methodName, bindingFlags, null, CallingConventions.Any, new Type[0], null);
            if (methodInfo == null && bindingFlags.HasFlag(BindingFlags.NonPublic))
            {
                //NOTE: if a method is not found and we searching for a private method we should look into parent classes
                var baseType = targetType.BaseType;
                while (baseType != null)
                {
                    methodInfo = baseType.GetMethod(methodName, bindingFlags, null, CallingConventions.Any, new Type[0], null);
                    if (methodInfo != null)
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return methodInfo;
        }

        internal static PropertyInfo GetProperty(Type targetType, string propertyName)
        {
            return GetProperty(targetType, propertyName, allBindings);
        }

        internal static PropertyInfo GetProperty(Type targetType, string propertyName, BindingFlags bindingFlags)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            var property = targetType.GetProperty(propertyName, bindingFlags);
            if (property == null)
            {
                //NOTE: if a method is not found and we searching for a private method we should look into parent classes
                Type baseType = targetType.BaseType;
                while (baseType != null)
                {
                    property = baseType.GetProperty(propertyName, bindingFlags);
                    if (property != null)
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return property;
        }

        /// <summary>
        /// Tries to invoke parameterless method located in associated <see cref="Object"/>s.
        /// </summary>
        internal static bool TryInvokeMethod(string methodName, SerializedObject serializedObject)
        {
            var targetObjects = serializedObject.targetObjects;
            var method = GetMethod(methodName, targetObjects);
            if (method == null)
            {
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                return false;
            }

            for (var i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target == null)
                {
                    continue;
                }

                method.Invoke(target, null);
            }

            return true;
        }

        internal static object CreateInstance(Type targetType, bool forceUninitializedInstance)
        {
            if (targetType == null)
            {
                return null;
            }

            if (forceUninitializedInstance)
            {
                return FormatterServices.GetUninitializedObject(targetType);
            }

            if (targetType.IsValueType)
            {
                return Activator.CreateInstance(targetType);
            }

            var defaultConstructor = targetType.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor != null)
            {
                return Activator.CreateInstance(targetType);
            }

            return FormatterServices.GetUninitializedObject(targetType);
        }
    }
}