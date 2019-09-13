using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class BoxedGroupAttribute : ToolboxGroupAttribute
{
    public BoxedGroupAttribute(string groupName) : base(groupName)
    { }
}