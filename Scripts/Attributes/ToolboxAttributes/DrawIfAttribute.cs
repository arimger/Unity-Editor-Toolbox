using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DrawIfAttribute : ConditionAttribute
{
    public DrawIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
    { }
}