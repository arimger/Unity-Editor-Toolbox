using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ClampAttribute : PropertyAttribute
    {
        public ClampAttribute(float minValue, float maxValue)
        {
            MinValue = Mathf.Min(minValue, maxValue);
            MaxValue = Mathf.Max(maxValue, minValue);
        }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    }
}