using System;

namespace UnityEngine
{
    /// <summary>
    /// Enables serialized field if the provided condition is met. Conditional property has to be serialized.
    /// 
    /// <para>Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, any <see cref="Enum"/>, and <see cref="Object"/> (but has to be compared to a <see cref="bool"/> value).</para>
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnableIfAttribute : ComparisonAttribute
    {
        public EnableIfAttribute(string propertyName, object valueToMatch) : base(propertyName, valueToMatch)
        { }
    }
}