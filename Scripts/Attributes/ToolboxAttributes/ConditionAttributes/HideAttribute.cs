using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides serialized field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideAttribute : ToolboxConditionAttribute
    { }
}