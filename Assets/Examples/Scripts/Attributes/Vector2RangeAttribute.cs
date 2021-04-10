using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class Vector2RangeAttribute : PropertyAttribute
{
    public Vector2RangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float Min { get; private set; }
    public float Max { get; private set; }
}