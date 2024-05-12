using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws a information box if the associated value is null.
    /// 
    /// <para>Supported types: any <see cref="Object"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute() : this("Variable has to be assigned.")
        { }

        public NotNullAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}