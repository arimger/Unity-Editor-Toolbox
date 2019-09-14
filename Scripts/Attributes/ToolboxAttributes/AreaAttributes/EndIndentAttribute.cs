using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EndIndentAttribute : ToolboxAreaAttribute
{
    public EndIndentAttribute(int indentToSubtract = 1)
    {
        IndentToSubtract = indentToSubtract;
    }

    public int IndentToSubtract { get; private set; }
}
