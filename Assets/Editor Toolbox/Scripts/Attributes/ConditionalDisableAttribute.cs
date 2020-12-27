using System;

namespace UnityEngine
{
    /// <summary>
    /// Disables associated field if provided condition is met.
    /// Supported types: all.
    /// </summary>
    [Obsolete("Use DisablefIfAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConditionalDisableAttribute : ConditionalAttribute
    {
        public ConditionalDisableAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}