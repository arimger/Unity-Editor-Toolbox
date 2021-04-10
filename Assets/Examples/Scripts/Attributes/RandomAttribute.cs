using System;
using UnityEngine;

/// <summary>
/// Draws additional button which allows to recalculate the target value.
/// 
/// <para>Supported types: <see cref="int"/>, <see cref="float"/>, <see cref="double"/>.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class RandomAttribute : PropertyAttribute
{
    public RandomAttribute(float minValue, float maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public float MinValue { get; private set; }

    public float MaxValue { get; private set; }
}