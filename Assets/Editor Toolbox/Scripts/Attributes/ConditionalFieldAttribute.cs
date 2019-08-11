using System;

using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    public ConditionalFieldAttribute(string propertyToCheck, object compareValue = null)
    {
        PropertyToCheck = propertyToCheck;
        CompareValue = compareValue;
    }

    public string PropertyToCheck { get; private set; }

    public object CompareValue { get; private set; }
}