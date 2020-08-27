using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws the left-ordered toggle.
    /// Supported types: <see cref="bool"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LeftToggleAttribute : PropertyAttribute
    { }
}