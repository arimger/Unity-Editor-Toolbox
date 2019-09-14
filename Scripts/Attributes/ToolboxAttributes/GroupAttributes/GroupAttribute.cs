using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : ToolboxGroupAttribute
{
    public GroupAttribute(string groupName) : base(groupName)
    { }
}