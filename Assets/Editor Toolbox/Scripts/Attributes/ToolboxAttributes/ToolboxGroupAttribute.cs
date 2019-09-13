using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public abstract class ToolboxGroupAttribute : ToolboxAttribute
{
    protected ToolboxGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }

    public string GroupName { get; protected set; }
}