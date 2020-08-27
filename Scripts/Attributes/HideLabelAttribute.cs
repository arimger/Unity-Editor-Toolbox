using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides property label.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideLabelAttribute : PropertyAttribute
    { }
}