using System.Reflection;

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
    }
}