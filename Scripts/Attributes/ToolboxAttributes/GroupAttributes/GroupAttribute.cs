using System;

[Obsolete("Work in progress. Use BeginGroupAttribute and EndGroupAttribute instead.")]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : ToolboxGroupAttribute
{
    public GroupAttribute(string groupName) : base(groupName)
    { }
}