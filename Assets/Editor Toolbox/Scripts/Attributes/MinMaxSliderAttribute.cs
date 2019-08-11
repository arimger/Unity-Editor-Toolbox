using System;

using UnityEngine;

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