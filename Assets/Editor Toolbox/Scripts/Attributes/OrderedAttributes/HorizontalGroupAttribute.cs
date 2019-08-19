using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class HorizontalGroupAttribute : OrderedGroupAttribute
{
    public HorizontalGroupAttribute(string groupName) : base(groupName)
    { }
}