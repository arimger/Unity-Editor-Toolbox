using System;

namespace UnityEngine
{
    [Obsolete("Use ShofIfAttribute instead.")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ConditionalShowAttribute : ConditionalAttribute
    {
        public ConditionalShowAttribute(string propertyToCheck, object compareValue = null) : base(propertyToCheck, compareValue)
        { }
    }
}