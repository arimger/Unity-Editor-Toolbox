using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : OrderedAttribute
{
    public GroupAttribute(string groupName)
    {
        GroupName = groupName;
    }

    public string GroupName { get; private set; }
}