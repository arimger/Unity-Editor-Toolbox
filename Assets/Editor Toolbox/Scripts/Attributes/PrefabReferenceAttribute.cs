using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only prefabs.
    /// Supported types: any <see cref="Object"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabReferenceAttribute : PropertyAttribute
    { }
}