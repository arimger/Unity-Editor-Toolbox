using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates a popup window with an input field. Allows to search for enum values by their name.
    /// 
    /// <para>Supported types: any <see cref="Enum"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class SearchableEnumAttribute : PropertyAttribute
    { }
}