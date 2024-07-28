using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Marks serialized field as read-only but only in the EditMode.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class DisableInEditModeAttribute : ToolboxConditionAttribute
    { }
}