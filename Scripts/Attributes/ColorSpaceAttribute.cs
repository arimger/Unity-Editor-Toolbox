using System;

using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ColorSpaceAttribute : PropertyAttribute
{
    public ColorSpaceAttribute(float spaceHeight, float lineHeight, float lineWidth, float r, float g, float b)
    {
        SpaceHeight = spaceHeight;
        LineHeight = lineHeight;
        LineWidth = lineWidth;

        LineColor = new Color(r, g, b);
    }

    public float SpaceHeight { get; private set; }
    public float LineHeight { get; private set; }
    public float LineWidth { get; private set; }

    public Color LineColor { get; private set; }
}