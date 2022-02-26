using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Validates if current value is a proper Scene name.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class SceneNameAttribute : PropertyAttribute
    { }
}