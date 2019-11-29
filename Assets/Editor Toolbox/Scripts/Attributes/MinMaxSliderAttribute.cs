using System;

namespace UnityEngine
{
    /// <summary>
    /// Attribute will draw min-max slider based on <see cref="Vector2"/> value.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public MinMaxSliderAttribute(float minValue, float maxValue)
        {
            MinValue = Mathf.Min(minValue, maxValue);
            MaxValue = Mathf.Max(maxValue, minValue);
        }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    }
}