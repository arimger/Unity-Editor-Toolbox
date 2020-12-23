using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks associated field as read-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableInPlayModeAttribute : ToolboxConditionAttribute
    { }
}