using System;

namespace UnityEngine
{
    /// <summary>
    /// Enables associated field if provided condition is met.
    /// Supported types: all.
    /// </summary>
    [Obsolete("Use EnableIfAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConditionalEnableAttribute : ConditionalAttribute
    {
        public ConditionalEnableAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}