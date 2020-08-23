using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks associated field as read-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DisableAttribute : ToolboxConditionAttribute
    { }
}