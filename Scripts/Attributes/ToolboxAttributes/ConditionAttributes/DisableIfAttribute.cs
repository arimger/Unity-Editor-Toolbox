using System;

namespace UnityEngine
{
    /// <summary>
    /// Disables property if provided condition is met. Conditional property has to be serialized.
    /// Supported condition types: <see cref="bool"/>, <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/>, any <see cref="Enum"/>, and <see cref="Object"/> (but has to be compared to a <see cref="bool"/> value). 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableIfAttribute : ComparisonAttribute
    {
        public DisableIfAttribute(string comparedPropertyName, object targetValue) : base(comparedPropertyName, targetValue)
        { }
    }
}