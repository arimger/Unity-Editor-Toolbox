using System;

namespace UnityEngine
{
    /// <summary>
    /// Shows property if provided condition is met. Conditional property has to be serialized.
    /// Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/> and any <see cref="Enum"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : ComparisonAttribute
    {
        public ShowIfAttribute(string comparedPropertyName, object targetValue) : base(comparedPropertyName, targetValue)
        { }
    }
}