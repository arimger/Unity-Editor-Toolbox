using System;

namespace UnityEngine
{
    /// <summary>
    /// Ignores parent label and default foldout for children-based properties.
    /// 
    /// <para>Supported types: any.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IgnoreParentAttribute : ToolboxSelfPropertyAttribute
    { }
}