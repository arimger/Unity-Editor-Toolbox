using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws color picker and sets color hex code.
    /// Supported types: <see cref="string"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HexColorAttribute : PropertyAttribute
    { }
}