using System;

using UnityEngine;

[AttributeUsage(validOn:AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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