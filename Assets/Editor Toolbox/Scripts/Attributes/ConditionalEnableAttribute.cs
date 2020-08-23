using System;

namespace UnityEngine
{
    [Obsolete("Use EnableIfAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ConditionalEnableAttribute : ConditionalAttribute
    {
        public ConditionalEnableAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}