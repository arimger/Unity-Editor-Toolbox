using System;

namespace UnityEngine
{
    /// <summary>
    /// Hides property if provided condition is not met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class HideIfAttribute : ConditionAttribute
    {
        public HideIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
        { }
    }
}