using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws a password field.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PasswordAttribute : PropertyAttribute
    { }
}