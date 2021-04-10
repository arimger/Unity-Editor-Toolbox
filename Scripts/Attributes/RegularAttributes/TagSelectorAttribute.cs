using System;

namespace UnityEngine
{
    /// <summary>
    /// Allows to pick a built-in tag value.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TagSelectorAttribute : PropertyAttribute
    { }
}