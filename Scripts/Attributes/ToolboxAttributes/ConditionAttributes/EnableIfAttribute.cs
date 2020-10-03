using System;

namespace UnityEngine
{
    /// <summary>
    /// Enables property if provided condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnableIfAttribute : ComparisonAttribute
    {
        public EnableIfAttribute(string comparedPropertyName, object targetValue) : base(comparedPropertyName, targetValue)
        { }
    }
}
