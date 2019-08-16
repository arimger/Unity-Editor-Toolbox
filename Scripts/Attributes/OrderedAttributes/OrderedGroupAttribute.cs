using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public abstract class OrderedGroupAttribute : OrderedAttribute
{
    protected OrderedGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }

    public string GroupName { get; protected set; }
}