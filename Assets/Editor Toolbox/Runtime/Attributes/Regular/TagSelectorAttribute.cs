using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick a built-in tag value.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class TagSelectorAttribute : PropertyAttribute
    { }
}