using System;

namespace UnityEngine
{
    /// <summary>
    /// Will clamp a numeric value between given min and max.
    /// 
    /// <para>Supported types: <see cref="int"/>, <see cref="float"/>, <see cref="double"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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