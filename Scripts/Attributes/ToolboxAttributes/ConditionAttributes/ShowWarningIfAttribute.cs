using System;

namespace UnityEngine
{
    /// <summary>
    /// Shows warning message if the provided condition is met. Conditional property has to be serialized.
    /// 
    /// <para>Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, any <see cref="Enum"/>, and <see cref="Object"/> (but has to be compared to a <see cref="bool"/> value).</para>
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowWarningIfAttribute : ComparisonAttribute
    {
        public ShowWarningIfAttribute(string propertyName, object valueToMatch, string message) : base(propertyName, valueToMatch)
        {
            Message = message;
        }

        public string Message { get; private set; }

        public bool DisableField { get; set; } = true;
    }
}