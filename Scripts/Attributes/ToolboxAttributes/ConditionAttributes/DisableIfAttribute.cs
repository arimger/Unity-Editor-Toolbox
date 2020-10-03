using System;

namespace UnityEngine
{    
    /// <summary>
    /// Disables property if provided condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableIfAttribute : ComparisonAttribute
    {
        public DisableIfAttribute(string comparedPropertyName, object targetValue) : base(comparedPropertyName, targetValue)
        { }
    }
}