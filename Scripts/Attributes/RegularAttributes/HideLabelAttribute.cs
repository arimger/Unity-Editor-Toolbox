using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides property label.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideLabelAttribute : PropertyAttribute
    { }
}