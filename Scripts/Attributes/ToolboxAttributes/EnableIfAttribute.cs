using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EnableIfAttribute : ConditionAttribute
{
    public EnableIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
    { }
}
