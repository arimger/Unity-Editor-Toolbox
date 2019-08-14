using System;

using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
public class NotNullAttribute : PropertyAttribute
{
    public NotNullAttribute()
    {
        Label = "Variable has to be assigned.";
    }

    public NotNullAttribute(string label)
    {
        Label = label;
    }

    public string Label { get; private set; }
}