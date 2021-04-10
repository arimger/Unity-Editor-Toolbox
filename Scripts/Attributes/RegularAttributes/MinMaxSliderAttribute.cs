using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates min-max slider.
    /// 
    /// <para>Supported types: <see cref="Vector2"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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