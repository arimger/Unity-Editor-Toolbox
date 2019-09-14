using System;

/// <summary>
/// Begins horizontal group in inspector. Use <seealso cref="EndHorizontalAttribute"/> on serialized property to end this group.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class BeginHorizontalAttribute : ToolboxAreaAttribute
{
    public BeginHorizontalAttribute(float labelWidth = 35.0f)
    {
        LabelWdith = labelWidth;
    }

    public float LabelWdith { get; private set; }
}
