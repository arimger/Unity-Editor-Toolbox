using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only children (related to the target component).
    /// Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChildObjectOnlyAttribute : PropertyAttribute
    { }
}