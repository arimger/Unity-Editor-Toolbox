using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only prefabs.
    /// Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabReferenceAttribute : PropertyAttribute
    { }
}