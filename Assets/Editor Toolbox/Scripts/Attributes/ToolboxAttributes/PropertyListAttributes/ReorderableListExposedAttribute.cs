using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <inheritdoc cref="ReorderableListAttribute"/>
    /// <remarks>Works in the same way like <see cref="ReorderableListAttribute"/> but additionally allows to override some internal callbacks.</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ReorderableListExposedAttribute : ReorderableListAttribute
    {
        /// <summary>
        /// Name of the method that should be called every time new element is added to the list.
        /// </summary>
        public string OverrideNewElementMethodName { get; set; }
    }
}