using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides serialized field.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideAttribute : ToolboxConditionAttribute
    { }
}