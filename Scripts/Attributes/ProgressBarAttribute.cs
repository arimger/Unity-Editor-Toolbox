using System;

namespace UnityEngine
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ProgressBarAttribute : PropertyAttribute
    {
        public ProgressBarAttribute(string name = "", float minValue = 0, float maxValue = 100)
        {
            Name = name;

            MinValue = Mathf.Min(minValue, maxValue);
            MaxValue = Mathf.Max(maxValue, minValue);
        }

        public string Name { get; private set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    }
}