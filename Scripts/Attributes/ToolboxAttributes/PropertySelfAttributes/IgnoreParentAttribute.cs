using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Ignores parent label and default foldout for children-based properties.
    /// 
    /// <para>Supported types: any.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class IgnoreParentAttribute : ToolboxSelfPropertyAttribute
    { }
}