using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Creates dedicated drawer for fields marked with the <see cref="SerializeReference"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ReferencePickerAttribute : PropertyAttribute
    { }
}