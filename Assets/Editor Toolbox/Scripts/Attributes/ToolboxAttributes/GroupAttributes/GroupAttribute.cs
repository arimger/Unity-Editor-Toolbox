using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : ToolboxGroupAttribute
{
    [Obsolete("Work in progress. Use BeginGroupAttribute and EndGroupAttribute instead.")]
    public GroupAttribute(string groupName) : base(groupName)
    { }
}