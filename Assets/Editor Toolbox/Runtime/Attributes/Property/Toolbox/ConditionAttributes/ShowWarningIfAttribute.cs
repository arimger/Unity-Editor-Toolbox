using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Shows warning message if the provided condition is met.
    /// 
    /// <para>Supported sources: fields, properties, and methods.</para>
    /// <para>Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, any <see cref="Enum"/>, and <see cref="Object"/> (but has to be compared to a <see cref="bool"/> value).</para>
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ShowWarningIfAttribute : ComparisonAttribute
    {
        public ShowWarningIfAttribute(string sourceHandle, object valueToMatch, string message) : base(sourceHandle, valueToMatch)
        {
            Message = message;
        }

        public string Message { get; private set; }

        /// <summary>
        /// Indicates if the targeted field should be read-only whenever the given condition is met.
        /// </summary>
        public bool DisableField { get; set; } = true;
    }
}