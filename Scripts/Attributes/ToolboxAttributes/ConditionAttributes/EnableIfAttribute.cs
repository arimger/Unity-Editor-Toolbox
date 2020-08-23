using System;

namespace UnityEngine
{
    /// <summary>
    /// Enables property if provided condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnableIfAttribute : ComparisonAttribute
    {
        public EnableIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
        { }
    }
}
