using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws a password field.
    /// Supported types: <see cref="string"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PasswordAttribute : PropertyAttribute
    { }
}