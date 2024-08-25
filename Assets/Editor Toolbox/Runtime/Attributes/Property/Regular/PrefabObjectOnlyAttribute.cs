using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only prefabs.
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class PrefabObjectOnlyAttribute : PropertyAttribute
    { }
}