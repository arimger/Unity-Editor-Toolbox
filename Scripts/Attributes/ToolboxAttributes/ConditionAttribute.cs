using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public abstract class ConditionAttribute : ToolboxAttribute
{
    public ConditionAttribute(string comparedPropertyName, object comparedValue)
    {
        ComparedPropertyName = comparedPropertyName;
        ComparedValue = comparedValue;
    }

    public string ComparedPropertyName { get; private set; }

    public object ComparedValue { get; private set; }
}
