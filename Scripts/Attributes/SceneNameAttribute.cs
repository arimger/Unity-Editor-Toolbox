using System;

namespace UnityEngine
{
    /// <summary>
    /// Validates if current value is a proper Scene name.
    /// Supported types: <see cref="string"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SceneNameAttribute : PropertyAttribute
    { }
}