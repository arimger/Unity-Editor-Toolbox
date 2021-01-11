using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks associated field as read-only.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableAttribute : ToolboxConditionAttribute
    { }
}