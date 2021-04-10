using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only prefabs.
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabObjectOnlyAttribute : PropertyAttribute
    { }
}