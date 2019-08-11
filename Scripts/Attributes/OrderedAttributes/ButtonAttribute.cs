using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ButtonAttribute : OrderedAttribute
{
    public ButtonAttribute(string label = null)
    {
        Label = label;
    }

    public string Label { get; private set; }
}