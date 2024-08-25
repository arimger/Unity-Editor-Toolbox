using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates layer-based popup menu.
    /// 
    /// <para>Supported types: <see cref="int"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class LayerAttribute : PropertyAttribute
    { }
}