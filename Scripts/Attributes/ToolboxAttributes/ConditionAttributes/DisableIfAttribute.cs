using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DisableIfAttribute : ConditionAttribute
{
    public DisableIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
    { }
}
