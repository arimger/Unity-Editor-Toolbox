using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    internal static class ReflectionUtility
    {
        private readonly static Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;


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
            return GetObjectMethod(methodName, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, 
                targetObjects);
        }

        internal static MethodInfo GetObjectMethod(string methodName, BindingFlags bindingFlags, params Object[] targetObjects)
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return null;
            }

            var targetType = targetObjects[0].GetType();
            var methodInfo = targetType.GetMethod(methodName, bindingFlags);
            return methodInfo;
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