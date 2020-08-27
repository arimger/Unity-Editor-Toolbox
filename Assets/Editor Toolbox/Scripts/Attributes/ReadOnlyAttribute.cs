using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks field as read-only.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyFieldAttribute : PropertyAttribute
    { }
}