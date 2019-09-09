using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EnableIfAttribute : ConditionIfAttribute
{
    public EnableIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
    { }
}
