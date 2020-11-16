using System;

namespace UnityEngine
{    
    /// <summary>
    /// Accepts only child <see cref="GameObject"/>s and <see cref="Component"/>s.
    /// Supported types: any <see cref="Object"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChildObjectOnlyAttribute : PropertyAttribute
    { }
}