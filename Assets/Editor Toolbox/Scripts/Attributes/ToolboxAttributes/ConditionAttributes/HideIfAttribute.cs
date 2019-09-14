using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class HideIfAttribute : ConditionAttribute
{
    public HideIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
    { }
}
