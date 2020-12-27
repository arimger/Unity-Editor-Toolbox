using System;

namespace UnityEngine
{
    /// <summary>
    /// Shows associated field if provided condition is met.
    /// Supported types: all.
    /// </summary>
    [Obsolete("Use ShowfIfAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ConditionalShowAttribute : ConditionalAttribute
    {
        public ConditionalShowAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}