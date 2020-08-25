using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks field as read-only.
    /// Use <see cref="DisableAttribute"/> to work together with any additional <see cref="ToolboxPropertyAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyFieldAttribute : PropertyAttribute
    { }
}