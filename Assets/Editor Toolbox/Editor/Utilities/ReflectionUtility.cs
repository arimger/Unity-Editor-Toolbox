using System;
using System.Reflection;

using UnityEditor;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    internal static class ReflectionUtility
    {
        private readonly static Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

        public const BindingFlags allBindings = BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;


        /// <summary>
        /// Returns <see cref="MethodInfo"/> of the searched method within the Editor <see cref="Assembly"/>.
        /// </summary>
        internal static MethodInfo GetEditorMethod(string classType, string methodName, BindingFlags falgs)
        {
            return editorAssembly.GetType(classType).GetMethod(methodName, falgs);
        }

        internal static MethodInfo GetObjectMethod(string methodName, SerializedObject serializedObject)
        {
            return GetObjectMethod(methodName, serializedObject.targetObjects);
        }

        internal static MethodInfo GetObjectMethod(string methodName, params Object[] targetObjects)
        {
            return GetObjectMethod(methodName, allBindings, targetObjects);
        }

        internal static MethodInfo GetObjectMethod(string methodName, BindingFlags bindingFlags, params Object[] targetObjects)
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return null;
            }

            var targetType = targetObjects[0].GetType();
            var methodInfo = GetObjectMethod(targetType, methodName, bindingFlags);
            if (methodInfo == null && bindingFlags.HasFlag(BindingFlags.NonPublic))
            {
                //NOTE: if a method is not found and we searching for a private method we should look into parent classes
                var baseType = targetType.BaseType;
                while (baseType != null)
                {
                    methodInfo = GetObjectMethod(baseType, methodName, bindingFlags);
                    if (methodInfo != null)
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return methodInfo;
        }

        internal static MethodInfo GetObjectMethod(Type targetType, string methodName, BindingFlags bindingFlags)
        {
            return targetType.GetMethod(methodName, bindingFlags, null, CallingConventions.Any, new Type[0], null);
        }

        /// <summary>
        /// Tries to invoke parameterless method located in associated <see cref="Object"/>s.
        /// </summary>
        internal static bool TryInvokeMethod(string methodName, SerializedObject serializedObject)
        {
            var targetObjects = serializedObject.targetObjects;
            var method = GetObjectMethod(methodName, targetObjects);
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
    }
}