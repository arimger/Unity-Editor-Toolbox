using System;

namespace UnityEngine
{
    /// <summary>
    /// Marks serialized field as read-only but only in the PlayMode.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableInPlayModeAttribute : ToolboxConditionAttribute
    { }
}