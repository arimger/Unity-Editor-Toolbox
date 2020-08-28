using System;

namespace UnityEngine
{
    /// <summary>
    /// Shows property if provided condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : ComparisonAttribute
    {
        public ShowIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
        { }
    }
}