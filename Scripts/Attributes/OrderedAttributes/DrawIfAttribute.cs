using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DrawIfAttribute : OrderedAttribute
{
    public DrawIfAttribute(string comparedPropertyName, object comparedValue)
    {
        ComparedPropertyName = comparedPropertyName;
        ComparedValue = comparedValue;
    }

    public string ComparedPropertyName { get; private set; }

    public object ComparedValue { get; private set; }
}