using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Validates input values and accepts only objects that are not Prefabs.
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class NotPrefabObjectOnlyAttribute : PropertyAttribute
    {
        public bool AllowInstancedPrefabs { get; set; }
    }
}