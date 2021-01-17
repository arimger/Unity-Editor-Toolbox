using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick a built-in tag value.
    /// Supported types: <see cref="string"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TagSelectorAttribute : PropertyAttribute
    { }
}