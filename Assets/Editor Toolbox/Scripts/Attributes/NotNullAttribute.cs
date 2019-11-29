using System;

namespace UnityEngine
{
    /// <summary>
    /// This attribute will cause an additional help box if an associated property is null. Allowed only on <see cref="Object"/> type properties.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute()
        {
            Label = "Variable has to be assigned.";
        }

        public NotNullAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}