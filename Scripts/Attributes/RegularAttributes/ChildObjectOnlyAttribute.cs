using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only children (related to the target component).
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChildObjectOnlyAttribute : PropertyAttribute
    { }
}