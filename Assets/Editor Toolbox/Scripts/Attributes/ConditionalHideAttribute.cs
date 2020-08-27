using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides associated field if provided condition is met.
    /// Supported types: all.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConditionalHideAttribute : ConditionalAttribute
    {
        public ConditionalHideAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}