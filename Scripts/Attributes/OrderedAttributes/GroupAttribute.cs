using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : OrderedGroupAttribute
{
    public GroupAttribute(string groupName) : base(groupName)
    { }
}