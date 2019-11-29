using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws classic <see cref="bool"/> based property in custom boxed style.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BoxedToggleAttribute : PropertyAttribute
    {
        public BoxedToggleAttribute()
        { }

        public BoxedToggleAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}