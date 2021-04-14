using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates layer-based popup menu.
    /// 
    /// <para>Supported types: <see cref="int"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LayerAttribute : PropertyAttribute
    { }
}