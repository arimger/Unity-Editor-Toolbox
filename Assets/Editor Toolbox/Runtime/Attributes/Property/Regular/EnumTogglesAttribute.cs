using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates a group of toggles instead of the built-in popup menu.
    /// 
    /// <para>Supported types: any <see cref="Enum"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EnumTogglesAttribute : PropertyAttribute
    {
        /// <summary>
        /// Minimum width of toggle created within the drawer.
        /// </summary>
        public float ToggleWidth { get; set; } = 85.0f;
        /// <summary>
        /// Height of a single toggle.
        /// </summary>
        public float ToggleHeight { get; set; } = 16.0f;
        /// <summary>
        /// Spacing between toggle buttons.
        /// </summary>
        public float ToggleSpacing { get; set; } = 2.0f;
    }
}